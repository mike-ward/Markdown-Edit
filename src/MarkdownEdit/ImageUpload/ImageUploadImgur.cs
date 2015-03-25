using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MarkdownEdit.ImageUpload
{
    public class ImageUploadImgur : IImageUpload
    {
        public async Task<string> UploadBytesAsync(byte[] imageBytes)
        {
            using (var w = new WebClient())
            {
                const string clientId = "68a0074c7783fd4";
                w.Headers.Add("Authorization", "Client-ID " + clientId);
                var values = new NameValueCollection
                {
                    {"image", Convert.ToBase64String(imageBytes)}
                };

                var response = await w.UploadValuesTaskAsync("https://api.imgur.com/3/upload.json", values);
                var json = Encoding.UTF8.GetString(response);
                dynamic model = JsonConvert.DeserializeObject(json);
                return model.data.link;
            }
        }
    }
}