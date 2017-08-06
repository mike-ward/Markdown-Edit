namespace Infrastructure
{
    public interface ITheme
    {
        string Name { get; set; }
        string EditorBackground { get; set; }
        string EditorForeground { get; set; }
        double Header1Height { get; set; }
        double Header2Height { get; set; }
        IHighlight HighlightHeading { get; set; }
        IHighlight HighlightEmphasis { get; set; }
        IHighlight HighlightStrongEmphasis { get; set; }
        IHighlight HighlightInlineCode { get; set; }
        IHighlight HighlightBlockCode { get; set; }
        IHighlight HighlightBlockQuote { get; set; }
        IHighlight HighlightLink { get; set; }
        IHighlight HighlightImage { get; set; }
        string SpellCheckError { get; set; }
    }
}