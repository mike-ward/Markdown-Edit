using MarkdownEdit.Models;

namespace MarkdownEdit.MarkdownConverters
{
    public class CMarkGitHub : IMarkdownConverter
    {
        public string ConvertToHtml(string markdown)
        {
            var startInfo = Markdown.ProcessStartInfo("cmark-gfm", "--hardbreaks -e table -e strikethrough -e autolink -e tagfilter", true);
            var result = Markdown.ResultFromExecuteProcess(markdown, startInfo);
            return result;
        }
    }
}
