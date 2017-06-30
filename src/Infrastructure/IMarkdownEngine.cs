namespace Infrastructure
{
    public interface IMarkdownEngine
    {
        string ToHtml(string text);
    }
}