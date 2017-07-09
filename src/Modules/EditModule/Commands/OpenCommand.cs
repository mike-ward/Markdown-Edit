using System;
using ICSharpCode.AvalonEdit;
using Infrastructure;
using Prism.Commands;

namespace EditModule.Commands
{
    public class OpenCommand : DelegateCommand<string>
    {
        public OpenCommand(ITextEditorComponent textEditor, IOpenSaveActions openSaveActions, INotify notify)
            : base(file => Execute(file, textEditor, openSaveActions, notify), uri => CanExecute())
        {
        }

        private static void Execute(string file, ITextEditorComponent textEditor, IOpenSaveActions openSaveActions, INotify notify)
        {
            try
            {
                var te = (TextEditor)textEditor;
                var text = openSaveActions.Open(file);
                te.Document.Text = text;
                te.Document.FileName = file;
                te.ScrollToHome();
                te.IsModified = false;

            }
            catch (Exception ex)
            {
                notify.Alert(ex.Message);
            }
        }

        private static bool CanExecute()
        {
            return true;
        }
    }
}
