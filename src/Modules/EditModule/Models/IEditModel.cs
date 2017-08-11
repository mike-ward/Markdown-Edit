using ICSharpCode.AvalonEdit;

namespace EditModule.Models
{
    public interface IEditModel
    {
        void NewCommandExecuted(TextEditor textEditor);
        void OpenCommandExecuted(TextEditor textEditor, string fileName = null);
        void SaveCommandExecuted(TextEditor textEditor);
        void SaveAsCommandExecuted(TextEditor textEditor);
    }
}