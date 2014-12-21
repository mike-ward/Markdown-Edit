using System;
using System.Windows.Documents;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Snippets;

namespace MarkdownEdit
{
    internal sealed class CustomTabCommand : ICommand
    {
        private readonly TextEditor _editor;
        private readonly ICommand _baseCommand;

        public CustomTabCommand(TextEditor editor, ICommand baseCommand)
        {
            _editor = editor;
            _baseCommand = baseCommand;
        }

        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (_editor.SelectionLength == 0)
            {
                var wordStart = FindPrevWordStart(_editor.Document, _editor.CaretOffset);
                if (wordStart >= 0)
                {
                    var word = _editor.Document.GetText(wordStart, _editor.CaretOffset - wordStart);
                    if (word == "now")
                    {
                        var snippet = new Snippet
                        {
                            Elements =
                            {
                                new SnippetTextElement {Text = "later"}
                            }
                        };
                        _editor.Document.Remove(wordStart, _editor.CaretOffset - wordStart);
                        snippet.Insert(_editor.TextArea);
                    }
                }
            }
            _baseCommand.Execute(parameter);
        }

        public static int FindPrevWordStart(ITextSource textSource, int offset)
        {
            return TextUtilities.GetNextCaretPosition(textSource, offset, LogicalDirection.Backward, CaretPositioningMode.WordStart);
        }
    }
}