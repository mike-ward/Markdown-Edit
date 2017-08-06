namespace Infrastructure
{
    public interface IHighlight
    {
        string Name { get; set; }
        string Background { get; set; }
        string Foreground { get; set; }
        string FontWeight { get; set; }
        string FontStyle { get; set; }
        bool Underline { get; set; }
    }
}