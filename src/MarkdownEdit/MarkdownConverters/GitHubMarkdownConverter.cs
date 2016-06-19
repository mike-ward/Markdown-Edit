using MarkdownEdit.Models;

namespace MarkdownEdit.MarkdownConverters
{
    internal class GitHubMarkdownConverter : IMarkdownConverter
    {
        public string ConvertToHtml(string markdown)
        {
            return Markdown.Pandoc(markdown, "-f markdown_github-emoji+tex_math_dollars -t html5 --email-obfuscation=none --mathjax");
        }
    }
}