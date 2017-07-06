using System;
using EditModule.ViewModels;
using Infrastructure;
using Prism.Commands;

namespace EditModule.Commands
{
    public class SaveCommand : DelegateCommand
    {
        public SaveCommand(EditControlViewModel editControlViewModel, IOpenSaveActions openSaveActions, INotify notify)
            : base(() => Execute(editControlViewModel, openSaveActions, notify), () => CanExecute(editControlViewModel))
        {
        }

        private static void Execute(EditControlViewModel editControlViewModel, IOpenSaveActions openSaveActions, INotify notify)
        {
            try
            {
                if (string.IsNullOrEmpty(editControlViewModel.TextEditor.Document.FileName))
                {
                    var saveAsUri = openSaveActions.SaveAsDialog();
                    if (saveAsUri == null) return;
                    editControlViewModel.TextEditor.Document.FileName = saveAsUri.AbsolutePath;
                }
                openSaveActions.Save(new Uri(editControlViewModel.TextEditor.Document.FileName), editControlViewModel.TextEditor.Document.Text);
                editControlViewModel.IsDocumentModified = false;
            }
            catch (Exception ex)
            {
                notify.Alert(ex.Message);
            }
        }

        private static bool CanExecute(EditControlViewModel editControlViewModel)
        {
            return editControlViewModel.IsDocumentModified || string.IsNullOrEmpty(editControlViewModel.TextEditor.Document.FileName);
        }
    }
}
