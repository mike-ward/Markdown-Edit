using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MarkdownEdit.Models
{
    internal static class Version
    {
        public const string VersionNumber = "1.25.1";

        public static async Task<bool> IsCurrentVersion()
        {
            try
            {
                using (var http = new HttpClient())
                {
                    var version = await http.GetStringAsync("http://markdownedit.com/version.txt");
                    return string.IsNullOrWhiteSpace(version) || version == VersionNumber;
                }
            }
            catch (Exception)
            {
                return true;
            }
        }
    }
}