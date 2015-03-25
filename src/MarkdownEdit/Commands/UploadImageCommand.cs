using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;
using MarkdownEdit.ImageUpload;

namespace MarkdownEdit.Commands
{
    public class UploadImageCommand : ICommand
    {
        private readonly TextEditor _editor;
        private readonly IImageUpload _loader;

        public UploadImageCommand(TextEditor editor, IImageUpload loader)
        {
            _editor = editor;
            _loader = loader;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (Clipboard.ContainsData(DataFormats.Text) == false) return;
            var path = Clipboard.GetText().Trim("\"".ToCharArray());
            _loader
                .UploadBytesAsync(File.ReadAllBytes(path))
                .ContinueWith(task => _editor.Dispatcher.InvokeAsync(
                    () =>_editor.Document.Insert(_editor.CaretOffset, string.Format("![img]({0})", task.Result))));
        }

        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }
    }
}