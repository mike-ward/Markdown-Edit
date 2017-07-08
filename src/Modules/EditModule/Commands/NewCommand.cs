using System.Windows;
using ICSharpCode.AvalonEdit;
using Infrastructure;
using Prism.Commands;

namespace EditModule.Commands
{
    public class NewCommand : DelegateCommand
    {
        public NewCommand(IOpenSaveActions openSaveActions, ITextEditorComponent textEditor)
            : base(() => Execute(openSaveActions, textEditor))
        {
        }

        public static void Execute(IOpenSaveActions openSaveActions, ITextEditorComponent textEditor)
        {
            var te = (TextEditor)textEditor;
            if (te.IsModified)
            {
                if (openSaveActions.PromptToSave(te.Document.FileName, te.Text) == MessageBoxResult.Cancel) return;
            }
            te.Document.Text = string.Empty;
            te.Document.FileName = string.Empty;
            te.IsModified = false;
        }
    }
}
