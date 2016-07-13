using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MarkdownEdit.Models
{
    internal static class Version
    {
        public const string VersionNumber = "1.28.2";

        public static async Task<bool> IsCurrentVersion()
        {
            try
            {
                using (var http = new HttpClient())
                {
                    var version = await http.GetStringAsync("http://markdownedit.com/version.txt");
                    return !version.All(c => c >= '0' && c <= '9' || c == '.')
                           || string.IsNullOrWhiteSpace(version)
                           || version == VersionNumber;
                }
            }
            catch (Exception)
            {
                return true;
            }
        }
    }
}