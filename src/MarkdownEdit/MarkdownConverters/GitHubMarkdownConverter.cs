using MarkdownEdit.Models;

namespace MarkdownEdit.MarkdownConverters
{
    internal class GitHubMarkdownConverter : IMarkdownConverter
    {
        public string ConvertToHtml(string markdown)
        {
            return ConvertText.Pandoc(markdown, "-f markdown_github -t html5 --email-obfuscation=none");
        }
    }
}