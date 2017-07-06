using System;
using EditModule.ViewModels;
using ICSharpCode.AvalonEdit;
using Infrastructure;
using Prism.Commands;

namespace EditModule.Commands
{
    public class OpenCommand : DelegateCommand<Uri>
    {
        public OpenCommand(EditControlViewModel editControlViewModel, IOpenSaveActions openSaveActions, INotify notify)
            : base(uri => Execute(uri, editControlViewModel, openSaveActions, notify), uri => CanExecute())
        {
        }

        private static void Execute(Uri file, EditControlViewModel editControlViewModel, IOpenSaveActions openSaveActions, INotify notify)
        {
            try
            {
                var text = openSaveActions.Open(file);
                editControlViewModel.TextEditor.Document.Text = text;
                editControlViewModel.TextEditor.Document.FileName = file.ToString();
                ((TextEditor)editControlViewModel.TextEditor).ScrollToHome();
                editControlViewModel.IsDocumentModified = false;

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
