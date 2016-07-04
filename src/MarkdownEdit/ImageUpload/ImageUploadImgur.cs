using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MarkdownEdit.ImageUpload
{
    public class ImageUploadImgur : IImageUpload
    {
        public async Task<string> UploadBytesAsync(
            byte[] imageBytes,
            UploadProgressChangedEventHandler progress = null,
            UploadValuesCompletedEventHandler completed = null)
        {
            using (var webClient = new WebClient())
            {
                try
                {
                    const string clientId = "68a0074c7783fd4";
                    webClient.Headers.Add("Authorization", "Client-ID " + clientId);
                    var values = new NameValueCollection
                    {
                        {"image", Convert.ToBase64String(imageBytes)}
                    };

                    if (progress != null) webClient.UploadProgressChanged += progress;
                    if (completed != null) webClient.UploadValuesCompleted += completed;

                    var response = await webClient.UploadValuesTaskAsync("https://api.imgur.com/3/upload.json", values);
                    var json = Encoding.UTF8.GetString(response);
                    dynamic model = JsonConvert.DeserializeObject(json);
                    return (bool)model.success ? (string)model.data.link : (string)model.data.error;
                }
                catch (Exception e)
                {
                    return e.Message;
                }
                finally
                {
                    if (progress != null) webClient.UploadProgressChanged -= progress;
                    if (completed != null) webClient.UploadValuesCompleted -= completed;
                }
            }
        }
    }
}