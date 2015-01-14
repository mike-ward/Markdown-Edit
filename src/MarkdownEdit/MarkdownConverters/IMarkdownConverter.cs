namespace MarkdownEdit.MarkdownConverters
{
    public interface IMarkdownConverter
    {
        string ConvertToHtml(string markdown, bool resolveUrls = true);
    }
}