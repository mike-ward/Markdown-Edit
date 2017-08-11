namespace Infrastructure
{
    public interface IMarkdownEngine
    {
        string Name { get; }
        string ToHtml(string text);
    }
}