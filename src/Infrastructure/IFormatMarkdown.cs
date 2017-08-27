namespace Infrastructure
{
    public interface IFormatMarkdown
    {
        string Format(string text);
        string Unformat(string text);
    }
}