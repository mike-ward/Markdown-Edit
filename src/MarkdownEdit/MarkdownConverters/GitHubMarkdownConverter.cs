using MarkdownEdit.Models;

namespace MarkdownEdit.MarkdownConverters
{
    internal class GitHubMarkdownConverter : IMarkdownConverter
    {
        public string ConvertToHtml(string markdown, bool resolveUrls = false)
        {
            return ConvertText.Pandoc(markdown, "-f markdown_github -t html5 --email-obfuscation=none");
        }
    }
}