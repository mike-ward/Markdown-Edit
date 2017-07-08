using System.Windows;
using ICSharpCode.AvalonEdit;
using Infrastructure;
using Prism.Commands;

namespace EditModule.Commands
{
    public class OpenDialogCommand : DelegateCommand
    {
        public OpenDialogCommand(ITextEditorComponent textEditor, OpenCommand openCommand, IOpenSaveActions openSaveActions)
            : base(() => Execute(textEditor, openCommand, openSaveActions))
        {
        }

        public static void Execute(ITextEditorComponent textEditor, OpenCommand openCommand, IOpenSaveActions openSaveActions)
        {
            var te = (TextEditor)textEditor;
            if (te.IsModified)
            {
                if (openSaveActions.PromptToSave(te.Document.FileName, te.Text) == MessageBoxResult.Cancel) return;
            }
            var file = openSaveActions.OpenDialog();
            if (file != null) openCommand.Execute(file);
        }
    }
}
