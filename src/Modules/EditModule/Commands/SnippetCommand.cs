using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using EditModule.ViewModels;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using Infrastructure;

namespace EditModule.Commands
{
    public class SnippetCommand : ICommand, IEditCommandHandler
    {
        private readonly ISnippetService _snippetService;
        private TextEditor _textEditor;
        private KeyBinding _oldTabBinding;
        public event EventHandler CanExecuteChanged { add { } remove { } }

        public SnippetCommand(ISnippetService snippetService)
        {
            _snippetService = snippetService;
        }

        public void Initialize(UIElement uiElement, EditControlViewModel viewModel)
        {
            _textEditor = viewModel.TextEditor;

            var editingKeyBindings = _textEditor.TextArea.DefaultInputHandler.Editing.InputBindings.OfType<KeyBinding>();
            _oldTabBinding = editingKeyBindings.Single(b => b.Key == Key.Tab && b.Modifiers == ModifierKeys.None);
            var newTabBinding = new KeyBinding(this, _oldTabBinding.Key, _oldTabBinding.Modifiers);
            _textEditor.TextArea.DefaultInputHandler.Editing.InputBindings.Remove(_oldTabBinding);
            _textEditor.TextArea.DefaultInputHandler.Editing.InputBindings.Add(newTabBinding);
            _snippetService.Initialize();
        }

        public void Execute(object sender, ExecutedRoutedEventArgs ea)
        {
            Execute(ea.Parameter);
        }

        public bool CanExecute(object parameter)
        {
            return _textEditor.TextArea != null;
        }

        public void Execute(object parameter)
        {
            if (_textEditor.SelectionLength == 0)
            {
                var wordStart = FindPrevWordStart(_textEditor.Document, _textEditor.CaretOffset);
                if (wordStart >= 0)
                {
                    var word = _textEditor.Document.GetText(wordStart, _textEditor.CaretOffset - wordStart);
                    var snippet = _snippetService.FindSnippet(word);
                    if (snippet != null)
                    {
                        _textEditor.Document.Remove(wordStart, _textEditor.CaretOffset - wordStart);
                        snippet.Insert(_textEditor.TextArea);
                        return;
                    }

                    var list = StartOfListOffset(_textEditor.Document, _textEditor.CaretOffset);
                    if (list > 0)
                    {
                        _textEditor.Document.Insert(list, "  ");
                        return;
                    }
                }
            }

            // Chain to original tab command handler when snippet not found
            _oldTabBinding.Command.Execute(parameter);
        }

        private static int FindPrevWordStart(ITextSource textSource, int offset)
        {
            var startOffset = offset;
            while (startOffset > 0 && char.IsWhiteSpace(textSource.GetCharAt(startOffset - 1)) == false)
            {
                startOffset -= 1;
            }
            return startOffset;
        }

        private static int StartOfListOffset(ITextSource textSource, int offset)
        {
            if (offset == 0) return -1;
            var startOffset = offset - 1;
            while (startOffset > 0 && char.IsWhiteSpace(textSource.GetCharAt(startOffset)))
            {
                startOffset = Math.Min(0, startOffset - 1);
            }
            var c = textSource.GetCharAt(startOffset);
            return c == '-' || c == '*' ? startOffset : -1;
        }
    }
}
