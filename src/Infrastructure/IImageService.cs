using System.Windows.Media.Imaging;

namespace Infrastructure
{
    public interface IImageService
    {
        string ImageFileToDataUri(string imageFile);
        string ClipboardDibToDataUri();
        BitmapSource ClipboardDibToBitmapSource();
        byte[] ToPngArray(BitmapSource bitmapsource);
        bool HasImageExtension(string url);
    }
}