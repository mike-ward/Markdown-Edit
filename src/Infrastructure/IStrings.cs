namespace Infrastructure
{
    public interface IStrings
    {
        // Editor
        string NewDocumentName { get; }
        string SaveYourChanges { get; }

        // Find Replace dialog
        string FindReplaceTabFind { get; }
        string FindReplaceTabReplace { get; }
        string FindReplaceWatermarkFind { get; }
        string FindReplaceWatermarkReplace { get; }
        string FindReplaceFind { get; }
        string FindReplaceReplace { get; }
        string FindReplaceReplaceAll { get; }
        string FindReplaceMatchCase { get; }
        string FindReplaceWholeWord { get; }
        string FindReplaceRegularExpression { get; }
        string FindReplaceWildCards { get; }
    }
}
