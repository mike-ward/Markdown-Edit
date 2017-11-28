using System;
using System.Windows;
using EditModule.ViewModels;
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
    }
}
