using System.Threading.Tasks;

namespace MarkdownEdit
{
    public interface IRemoteProvider
    {
        string DisplayName { get; }

        Task<object> GetCredentialsAsync();

        Task<string> OpenFilePickerAsync();

        Task OpenFileAsync(string file);

        Task SaveFileAsync(string file);
    }
}
