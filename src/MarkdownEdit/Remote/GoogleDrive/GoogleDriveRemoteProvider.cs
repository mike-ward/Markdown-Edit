using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Services;

namespace MarkdownEdit
{
    public class GoogleDriveRemoteProvider : IRemoteProvider
    {
        public async Task<object> GetCredentialsAsync()
        {
            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = "48620255894-51osrul13qj3gsjunvdsjtpljspn0l25.apps.googleusercontent.com",
                    ClientSecret = "cBG-6fUbBKjy-klv5zI2UKYr",
                },
                new[] { DriveService.Scope.Drive },
                "user",
                CancellationToken.None);
            return credential;
        }

        public Task OpenFileAsync(string file)
        {
            throw new NotImplementedException();
        }

        public async Task<string> OpenFilePickerAsync()
        {
            var credential = await GetCredentialsAsync();
            var service = DriveServiceFactory(credential as UserCredential);
            var files = await service.Files.List().ExecuteAsync();
            return files.Items[0].Title;
        }

        public Task SaveFileAsync(string file)
        {
            throw new NotImplementedException();
        }

        private static DriveService DriveServiceFactory(UserCredential credential)
        {
            var service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Markdown Edit"
            });
            return service;
        }
    }
}
