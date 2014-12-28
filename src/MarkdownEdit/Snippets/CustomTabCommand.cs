using System;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;

namespace MarkdownEdit
{
    internal sealed class CustomTabCommand : ICommand
    {
        private readonly TextEditor _editor;
        private readonly ICommand _baseCommand;
        private readonly ISnippetManager _snippetManager;

        public CustomTabCommand(TextEditor editor, ICommand baseCommand, ISnippetManager snippetManager)
        {
            _editor = editor;
            _baseCommand = baseCommand;
            _snippetManager = snippetManager;
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
                    var snippet = _snippetManager.FindSnippet(word);
                    if (snippet != null)
                    {
                        _editor.Document.Remove(wordStart, _editor.CaretOffset - wordStart);
                        snippet.Insert(_editor.TextArea);
                        return;
                    }
                }
            }
            _baseCommand.Execute(parameter);
        }

        public static int FindPrevWordStart(ITextSource textSource, int offset)
        {
            var startOffset = offset;
            while (startOffset > 0 && char.IsWhiteSpace((textSource.GetCharAt(startOffset - 1))) == false) startOffset -= 1;
            return startOffset;
        }
    }
}