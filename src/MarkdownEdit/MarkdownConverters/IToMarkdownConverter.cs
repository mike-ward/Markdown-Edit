namespace MarkdownEdit
{
    public interface IToMarkdownConverter
    {
        string ConvertToMarkdown(string from, string pathForRelativeUrls);
    }
}