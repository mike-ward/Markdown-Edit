using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace Infrastructure
{
    public interface ISpellCheckBackgroundRenderer : IBackgroundRenderer
    {
        TextSegmentCollection<TextSegment> ErrorSegments { get; }
    }
}