using System.Threading.Tasks;

namespace MarkdownEdit
{
    public interface IRemoteProvider
    {
        Task<object> GetCredentialsAsync();

        Task<string> OpenFilePickerAsync();

        Task OpenFileAsync(string file);

        Task SaveFileAsync(string file);
    }
}
