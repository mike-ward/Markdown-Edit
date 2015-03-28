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
            using (var w = new WebClient())
            {
                const string clientId = "68a0074c7783fd4";
                w.Headers.Add("Authorization", "Client-ID " + clientId);
                var values = new NameValueCollection
                {
                    {"image", Convert.ToBase64String(imageBytes)}
                };

                if (progress != null) w.UploadProgressChanged += progress;
                if (completed != null) w.UploadValuesCompleted += completed;

                var response = await w.UploadValuesTaskAsync("https://api.imgur.com/3/upload.json", values);
                var json = Encoding.UTF8.GetString(response);
                dynamic model = JsonConvert.DeserializeObject(json);

                if (progress != null) w.UploadProgressChanged -= progress;
                if (completed != null) w.UploadValuesCompleted -= completed;

                return model.data.link;
            }
        }
    }
}