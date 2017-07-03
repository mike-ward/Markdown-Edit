using System;
using ICSharpCode.AvalonEdit;
using Infrastructure;
using Prism.Commands;

namespace EditModule.Commands
{
    public class OpenCommand : DelegateCommand<Uri>
    {
        public OpenCommand(ITextEditorComponent textEditor, IFileActions fileActions)
            : base(uri => Execute(uri, textEditor, fileActions), uri => CanExecute())
        {
        }

        private static void Execute(Uri file, ITextEditorComponent textEdtior, IFileActions fileActions)
        {
            var text = fileActions.Open(file);
            textEdtior.Document.Text = text;
            textEdtior.Document.FileName = file.ToString();
        }

        private static bool CanExecute()
        {
            return true;
        }

    }
}
