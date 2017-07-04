using System;
using EditModule.ViewModels;
using Infrastructure;
using Prism.Commands;

namespace EditModule.Commands
{
    public class OpenCommand : DelegateCommand<Uri>
    {
        public OpenCommand(EditControlViewModel editControlViewModel, IFileActions fileActions)
            : base(uri => Execute(uri, editControlViewModel, fileActions), uri => CanExecute())
        {
        }

        private static void Execute(Uri file, EditControlViewModel editControlViewModel, IFileActions fileActions)
        {
            var text = fileActions.Open(file);
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
