namespace MarkdownEdit.MarkdownConverters
{
    public interface IToMarkdownConverter
    {
        string ConvertToMarkdown(string from, string pathForRelativeUrls);
    }
}