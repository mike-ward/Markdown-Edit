using ICSharpCode.AvalonEdit;

namespace Infrastructure
{
    public interface IEditService
    {
        FindReplaceOptions FindReplaceOptions { get; }
        void SelectWordAt(TextEditor editor, int offset);
        void AddRemoveText(TextEditor editor, string quote);
        void InsertBlockQuote(TextEditor textEditor);
        bool FindNext(TextEditor textEditor, FindReplaceOptions findReplaceOptions);
        void Replace(TextEditor textEditor, FindReplaceOptions findReplaceOptions);
        void ReplaceAll(TextEditor editor, FindReplaceOptions findReplaceOptions);
    }
}