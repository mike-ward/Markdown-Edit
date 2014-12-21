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
        private readonly TextEditor editor;
        private readonly ICommand baseCommand;

        public CustomTabCommand(TextEditor editor, ICommand baseCommand)
        {
            this.editor = editor;
            this.baseCommand = baseCommand;
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
            if (editor.SelectionLength == 0)
            {
                var wordStart = FindPrevWordStart(editor.Document, editor.CaretOffset);
                if (wordStart >= 0)
                {
                    var word = editor.Document.GetText(wordStart, editor.CaretOffset - wordStart);
                    if (word == "now")
                    {
                        var snippet = new Snippet
                        {
                            Elements =
                            {
                                new SnippetTextElement {Text = "later"}
                            }
                        };
                        editor.Document.Remove(wordStart, editor.CaretOffset - wordStart);
                        snippet.Insert(editor.TextArea);
                    }
                }
            }
            baseCommand.Execute(parameter);
        }

        public static int FindPrevWordStart(ITextSource textSource, int offset)
        {
            return TextUtilities.GetNextCaretPosition(textSource, offset, LogicalDirection.Backward, CaretPositioningMode.WordStart);
        }
    }
}