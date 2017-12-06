using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using EditModule.ViewModels;
using EditModule.Views;
using ICSharpCode.AvalonEdit;
using Infrastructure;

namespace EditModule.Features
{
    public class PasteEnhancements : IEditFeature
    {
        private readonly IImageService _imageService;
        private readonly IAbstractSyntaxTree _abstractSyntaxTree;
        private TextEditor _textEditor;

        public PasteEnhancements(IImageService imageService, IAbstractSyntaxTree abstractSyntaxTree)
        {
            _imageService = imageService;
            _abstractSyntaxTree = abstractSyntaxTree;
        }

        public void Initialize(EditControlViewModel viewModel)
        {
            _textEditor = viewModel.TextEditor;
            DataObject.AddPastingHandler(_textEditor, OnPaste);
            AllowImagePaste(_textEditor, _imageService);
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs pasteEventArgs)
        {
            var text = (string)pasteEventArgs.SourceDataObject.GetData(DataFormats.UnicodeText, true);
            if (string.IsNullOrEmpty(text)) return;
            string updatedText = null;
            
            if (Uri.IsWellFormedUriString(text, UriKind.Absolute))
            {
                var ast = _abstractSyntaxTree.GenerateAbstractSyntaxTree(_textEditor.Text);
                if (_abstractSyntaxTree.PositionSafeForSmartLink(ast, _textEditor.SelectionStart, _textEditor.SelectionLength))
                {
                    updatedText = _imageService.IsImageUrl(text.TrimEnd())
                        ? _imageService.CreateImageTag(text, _textEditor.SelectedText)
                        : string.IsNullOrEmpty(_textEditor.SelectedText) ? $"<{text}>" : $"[{_textEditor.SelectedText}]({text})";
                }
            }

            if (updatedText != null)
            {
                var dataObject = new DataObject();
                dataObject.SetData(DataFormats.UnicodeText, updatedText);
                pasteEventArgs.DataObject = dataObject;
            }
        }

        private static void AllowImagePaste(TextEditor editor, IImageService imageService)
        {
            // AvalonEdit only allows text paste. Hack the command to allow otherwise.
            var cmd = editor.TextArea.DefaultInputHandler.Editing.CommandBindings
                .FirstOrDefault(cb => cb.Command == ApplicationCommands.Paste);

            if (cmd == null) return;

            void CanExecute(object sender, CanExecuteRoutedEventArgs args) => 
                args.CanExecute = editor.TextArea?.Document != null && 
                editor.TextArea.ReadOnlySectionProvider.CanInsert(editor.TextArea.Caret.Offset);

            void Execute(object sender, ExecutedRoutedEventArgs args)
            {
                if (Clipboard.ContainsText())
                {
                    // WPF won't continue routing the command if there's PreviewExecuted handler.
                    // So, remove it, call Execute and reinstall the handler.
                    // Hack, hack hack...
                    try
                    {
                        cmd.PreviewExecuted -= Execute;
                        cmd.Command.Execute(args.Parameter);
                    }
                    finally
                    {
                        cmd.PreviewExecuted += Execute;
                    }
                }
                else if (Clipboard.ContainsImage())
                {
                    var dialog = new ImageDropDialog(editor, null)
                    {
                        Owner = Application.Current.MainWindow,
                        UseClipboard = true
                    };
                    dialog.ShowDialog();
                    args.Handled = true;
                }
            }

            cmd.CanExecute += CanExecute;
            cmd.PreviewExecuted += Execute;
        }
    }
}
