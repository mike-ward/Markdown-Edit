using System;
using EditModule.ViewModels;
using ICSharpCode.AvalonEdit;
using Infrastructure;
using Prism.Commands;

namespace EditModule.Commands
{
    public class SaveCommand : DelegateCommand
    {
        public SaveCommand(ITextEditorComponent textEditor, IOpenSaveActions openSaveActions, INotify notify)
            : base(() => Execute(textEditor, openSaveActions, notify), () => CanExecute(textEditor))
        {
        }

        private static void Execute(ITextEditorComponent textEditor, IOpenSaveActions openSaveActions, INotify notify)
        {
            try
            {
                if (string.IsNullOrEmpty(textEditor.Document.FileName))
                {
                    var fileName = openSaveActions.SaveAsDialog();
                    if (string.IsNullOrEmpty(fileName)) return;
                    textEditor.Document.FileName = fileName;
                }
                openSaveActions.Save(textEditor.Document.FileName, textEditor.Document.Text);
                ((TextEditor)textEditor).IsModified = false;
            }
            catch (Exception ex)
            {
                notify.Alert(ex.Message);
            }
        }

        private static bool CanExecute(ITextEditorComponent textEditor)
        {
            return ((TextEditor)textEditor).IsModified || string.IsNullOrEmpty(textEditor.Document.FileName);
        }
    }
}
