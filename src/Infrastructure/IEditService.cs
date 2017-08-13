using ICSharpCode.AvalonEdit;

namespace Infrastructure
{
    public interface IEditService
    {
        void SelectWordAt(TextEditor editor, int offset);
        void AddRemoveText(TextEditor editor, string quote);
    }
}