using System.Security.Cryptography.X509Certificates;

namespace MarkdownEdit
{
    public interface IToMarkdownConverter
    {
        string ConvertToMarkdown(string from, string pathForRelativeUrls);
    }
}