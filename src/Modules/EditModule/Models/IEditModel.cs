using ICSharpCode.AvalonEdit;

namespace EditModule.Models
{
    public interface IEditModel
    {
        void NewCommandHandler(TextEditor textEditor);
        void OpenCommandHandler(TextEditor textEditor, string fileName = null);
        void SaveCommandHandler(TextEditor textEditor);
        void SaveAsCommandHandler(TextEditor textEditor);
    }
}