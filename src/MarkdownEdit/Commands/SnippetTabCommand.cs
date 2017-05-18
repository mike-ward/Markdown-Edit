using System;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using MarkdownEdit.Snippets;

namespace MarkdownEdit.Commands
{
    internal sealed class SnippetTabCommand : ICommand
    {
        private readonly ICommand _baseCommand;
        private readonly TextEditor _editor;
        private readonly ISnippetManager _snippetManager;

        public SnippetTabCommand(TextEditor editor, ICommand baseCommand, ISnippetManager snippetManager)
        {
            _editor = editor;
            _baseCommand = baseCommand;
            _snippetManager = snippetManager;
        }

        public event EventHandler CanExecuteChanged { add { } remove { } }

        public bool CanExecute(object parameter)
        {
            return _editor.TextArea != null && _editor.TextArea.IsKeyboardFocused;
        }

        public void Execute(object parameter)
        {
            if (_editor.SelectionLength == 0)
            {
                var wordStart = FindPrevWordStart(_editor.Document, _editor.CaretOffset);
                if (wordStart >= 0)
                {
                    var word = _editor.Document.GetText(wordStart, _editor.CaretOffset - wordStart);
                    var snippet = _snippetManager.FindSnippet(word);
                    if (snippet != null)
                    {
                        _editor.Document.Remove(wordStart, _editor.CaretOffset - wordStart);
                        snippet.Insert(_editor.TextArea);
                        return;
                    }

                    var list = StartOfListOffset(_editor.Document, _editor.CaretOffset);
                    if (list > 0)
                    {
                        _editor.Document.Insert(list, "  ");
                        return;
                    }
                }
            }
            _baseCommand.Execute(parameter);
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
            return (c == '-' || c == '*') ? startOffset : -1;
        }
    }
}