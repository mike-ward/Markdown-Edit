using System;
using System.Threading.Tasks;

namespace MarkdownEdit
{
    public class GoogleDriveRemoteProvider : IRemoteProvider
    {
        public Task<object> GetCredentialsAsync()
        {
            throw new NotImplementedException();
        }

        public Task OpenFileAsync(string file)
        {
            throw new NotImplementedException();
        }

        public Task<string> OpenFilePickerAsync()
        {
            throw new NotImplementedException();
        }

        public Task SaveFileAsync(string file)
        {
            throw new NotImplementedException();
        }
    }
}
