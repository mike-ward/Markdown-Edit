using System;
using EditModule.ViewModels;
using Infrastructure;
using Prism.Commands;

namespace EditModule.Commands
{
    public class OpenCommand : DelegateCommand<Uri>
    {
        public OpenCommand(EditControlViewModel editControlViewModel, IOpenSaveActions openSaveActions)
            : base(uri => Execute(uri, editControlViewModel, openSaveActions), uri => CanExecute())
        {
        }

        private static void Execute(Uri file, EditControlViewModel editControlViewModel, IOpenSaveActions openSaveActions)
        {
            var text = openSaveActions.Open(file);
            editControlViewModel.TextEditor.Document.Text = text;
            editControlViewModel.TextEditor.Document.FileName = file.ToString();
            editControlViewModel.IsDocumentModified = false;
        }

        private static bool CanExecute()
        {
            return true;
        }
    }
}
