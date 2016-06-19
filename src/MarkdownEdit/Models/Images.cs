using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

// ReSharper disable InconsistentNaming
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable MemberCanBePrivate.Local

namespace MarkdownEdit.Models
{
    internal static class Images
    {
        public static string ImageFileToDataUri(string imageFile)
        {
            var ext = Path.GetExtension(imageFile)?.Replace(".", "");
            var bytes = File.ReadAllBytes(imageFile);
            return ImageBytesToDataUri(bytes, ext);
        }

        public static string ClipboardDibToDataUri()
        {
            var bitmapSource = ClipboardDibToBitmapSource();
            if (bitmapSource == null) return null;
            var bytes = ToPngArray(bitmapSource);
            return ImageBytesToDataUri(bytes, "png");
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            return codecs.FirstOrDefault(codec => codec.FormatID == format.Guid);
        }

        private static string ImageBytesToDataUri(byte[] bytes, string imageType)
        {
            return $"<img src=\"data:image/{imageType};base64,{Convert.ToBase64String(bytes)}\" />";
        }

        public static BitmapSource ClipboardDibToBitmapSource()
        {
            var ms = System.Windows.Clipboard.GetData("DeviceIndependentBitmap") as MemoryStream;
            if (ms == null) return null;

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
                bfOffBits = fileHeaderSize + infoHeaderSize + infoHeader.biClrUsed*4
            };

            var fileHeaderBytes = BinaryStructConverter.ToByteArray(fileHeader);
            var msBitmap = new MemoryStream();
            msBitmap.Write(fileHeaderBytes, 0, fileHeaderSize);
            msBitmap.Write(dibBuffer, 0, dibBuffer.Length);
            msBitmap.Seek(0, SeekOrigin.Begin);
            return BitmapFrame.Create(msBitmap); // frees stream when rendered 
        }

        public static byte[] ToPngArray(this BitmapSource bitmapsource)
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

        public static bool HasImageExtension(string url)
        {
            var trimUrl = url.TrimEnd();
            var imageExtensions = new[] {".jpg", "jpeg", ".png", ".gif"};
            return imageExtensions.Any(ext => trimUrl.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsImageUrl(string url)
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