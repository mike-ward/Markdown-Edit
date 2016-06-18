using System.Net;
using System.Threading.Tasks;

namespace MarkdownEdit.ImageUpload
{
    public interface IImageUpload
    {
        Task<string> UploadBytesAsync(
            byte[] imageBytes,
            UploadProgressChangedEventHandler progress = null,
            UploadValuesCompletedEventHandler completed = null);
    }
}