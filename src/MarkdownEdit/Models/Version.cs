using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

namespace MarkdownEdit.Models
{
    internal static class Version
    {
        public const string VersionNumber = "1.27";

        public static async Task<bool> IsCurrentVersion()
        {
            try
            {
                using (var http = new HttpClient())
                {
                    var version = await http.GetStringAsync("http://markdownedit.com/version.txt");
                    if (!version.All(c => c >= '0' && c <= '9' || c == '.')) return true; // network redirected to signon for instance
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