using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Infrastructure;
using Microsoft.Win32;

namespace ServicesModule.Services
{
    public class ImageService : IImageService
    {
        private readonly INotify _notify;
        private readonly IImageUpload _imageUpload;

        public ImageService(INotify notify, IImageUpload imageUpload)
        {
            _notify = notify;
            _imageUpload = imageUpload;
        }

        public async Task<string> UploadToImgur(
            Stream stream, 
            UploadProgressChangedEventHandler progress, 
            UploadValuesCompletedEventHandler completed)
        {
            try
            {
                var image = stream.ReadToArray();
                var link = await _imageUpload.UploadBytesAsync(image, progress, completed);
                if (Uri.IsWellFormedUriString(link, UriKind.Absolute)) return link;
                await _notify.Alert(link);
            }
            catch (Exception ex)
            {
                await _notify.Alert(ex.Message);
            }
            return null;
        }

        public async Task<string> ImageFileToDataUri(Stream stream, string imageType, string name)
        {
            var bytes = stream.ReadToArray();
            var dataUri = await Task.Factory.StartNew(() => ImageBytesToDataUri(bytes, imageType, name));
            return dataUri;
        }

        public async Task<string> SaveAs(Stream stream)
        {
            var dialog = new SaveFileDialog
            {
                OverwritePrompt = true,
                RestoreDirectory = true,
                Filter = "All files (*.*)|*.*"
            };

            var result = await Application.Current.Dispatcher.InvokeAsync(() => dialog.ShowDialog());
            if (result.HasValue && result.Value == false) return string.Empty;
            var fileName = dialog.FileName;
            if (string.IsNullOrEmpty(fileName)) return string.Empty;

            if (File.Exists(fileName))
            {
                // todo: localize
                var message = "Overwrite?";
                if (await _notify.ConfirmYesNo(message) != MessageBoxResult.Yes) return string.Empty;
            }

            File.WriteAllBytes(fileName, stream.ReadToArray());
            return fileName;
        }

        public string CreateImageTag(string link, string title)
        {
            return $"![{title}]({link})\n";
        }

        public string ClipboardDibToDataUri()
        {
            var bitmapSource = ClipboardDibToBitmapSource();
            if (bitmapSource == null) return null;
            var bytes = ToPngArray(bitmapSource);
            return ImageBytesToDataUri(bytes, "png", "clipboard");
        }

        private static string ImageBytesToDataUri(byte[] bytes, string imageType, string name)
        {
            return $"<img src=\"data:image/{imageType};base64,{Convert.ToBase64String(bytes)}\" alt=\"{name ?? string.Empty}\" />";
        }

        public BitmapSource ClipboardDibToBitmapSource()
        {
            if (!(Clipboard.GetData("DeviceIndependentBitmap") is MemoryStream ms)) return null;

            var dibBuffer = new byte[ms.Length];
            ms.Read(dibBuffer, 0, dibBuffer.Length);

            var infoHeader = BinaryStructConverter.FromByteArray<BITMAPINFOHEADER>(dibBuffer);
            var fileHeaderSize = Marshal.SizeOf(typeof(BITMAPFILEHEADER));
            var infoHeaderSize = infoHeader.biSize;
            var fileSize = fileHeaderSize + infoHeader.biSize + infoHeader.biSizeImage;

            var fileHeader = new BITMAPFILEHEADER
            {
                bfType = BITMAPFILEHEADER.BM,
                bfSize = fileSize,
                bfReserved1 = 0,
                bfReserved2 = 0,
                bfOffBits = fileHeaderSize + infoHeaderSize + infoHeader.biClrUsed * 4
            };

            var fileHeaderBytes = BinaryStructConverter.ToByteArray(fileHeader);
            var msBitmap = new MemoryStream();
            msBitmap.Write(fileHeaderBytes, 0, fileHeaderSize);
            msBitmap.Write(dibBuffer, 0, dibBuffer.Length);
            msBitmap.Seek(0, SeekOrigin.Begin);
            return BitmapFrame.Create(msBitmap); // frees stream when rendered 
        }

        public byte[] ToPngArray(BitmapSource bitmapsource)
        {
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new PngBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                outStream.Flush();
                return outStream.ToArray();
            }
        }

        public bool HasImageExtension(string url)
        {
            var trimUrl = url.TrimEnd();
            var imageExtensions = new[] { ".jpg", "jpeg", ".png", ".gif" };
            return imageExtensions.Any(ext => trimUrl.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsImageUrl(string url)
        {
            try
            {
                if (HasImageExtension(url)) return true;

                var request = WebRequest.Create(url);
                request.Method = "HEAD";
                request.Timeout = 2000;
                using (var response = request.GetResponse())
                    return response.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception)
            {
                return false;
            }
        }

        // ReSharper disable InconsistentNaming
        // ReSharper disable FieldCanBeMadeReadOnly.Local
        // ReSharper disable MemberCanBePrivate.Local

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        private struct BITMAPFILEHEADER
        {
            public static readonly short BM = 0x4d42; // BM
            public short bfType;
            public int bfSize;
            public short bfReserved1;
            public short bfReserved2;
            public int bfOffBits;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BITMAPINFOHEADER
        {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
        }

        public static class BinaryStructConverter
        {
            public static T FromByteArray<T>(byte[] bytes) where T : struct
            {
                var ptr = IntPtr.Zero;
                try
                {
                    var size = Marshal.SizeOf(typeof(T));
                    ptr = Marshal.AllocHGlobal(size);
                    Marshal.Copy(bytes, 0, ptr, size);
                    var obj = Marshal.PtrToStructure(ptr, typeof(T));
                    return (T)obj;
                }
                finally
                {
                    if (ptr != IntPtr.Zero) Marshal.FreeHGlobal(ptr);
                }
            }

            public static byte[] ToByteArray<T>(T obj) where T : struct
            {
                var ptr = IntPtr.Zero;
                try
                {
                    var size = Marshal.SizeOf(typeof(T));
                    ptr = Marshal.AllocHGlobal(size);
                    Marshal.StructureToPtr(obj, ptr, true);
                    var bytes = new byte[size];
                    Marshal.Copy(ptr, bytes, 0, size);
                    return bytes;
                }
                finally
                {
                    if (ptr != IntPtr.Zero) Marshal.FreeHGlobal(ptr);
                }
            }
        }
    }
}