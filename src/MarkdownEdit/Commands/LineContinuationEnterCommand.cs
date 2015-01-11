using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;

namespace MarkdownEdit
{
    public class LineContinuationEnterCommand : ICommand
    {
        private readonly TextEditor _editor;
        private readonly ICommand _baseCommand;
        private readonly Regex _orderedListPattern = new Regex(@"^[ ]{0,3}(\d+)\.(?=[ ]{1,3}\S)", RegexOptions.Compiled);
        private readonly Regex _orderedListEndPattern = new Regex(@"^[ ]{0,3}(\d+)\.(?=[ ]{1,3}\s*)", RegexOptions.Compiled);
        private readonly Regex _unorderedListPattern = new Regex(@"^[ ]{0,3}[-\*\+](?=[ ]{1,3}\S)", RegexOptions.Compiled);
        private readonly Regex _unorderedListEndPattern = new Regex(@"^[ ]{0,3}[-\*\+](?=[ ]{1,3}\s*)", RegexOptions.Compiled);
        private readonly Regex _blockQuotePattern = new Regex(@"^(([ ]{0,4}>)+)[ ]{0,4}.{2}", RegexOptions.Compiled);
        private readonly Regex _blockQuoteEndPattern = new Regex(@"^([ ]{0,4}>)+[ ]{0,4}\s*", RegexOptions.Compiled);

        public LineContinuationEnterCommand(TextEditor editor, ICommand baseCommand)
        {
            _editor = editor;
            _baseCommand = baseCommand;
        }

        public bool CanExecute(object parameter)
        {
            return _editor.TextArea != null && _editor.TextArea.IsKeyboardFocused;
        }

        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }

        public void Execute(object parameter)
        {
            _baseCommand.Execute(parameter);

            var document = _editor.Document;
            var line = document.GetLineByOffset(_editor.SelectionStart)?.PreviousLine;
            if (line == null) return;
            var text = document.GetText(line.Offset, line.Length);

            Func<Regex, Action<Match>, Action> matchDo = (pattern, action) =>
            {
                var match = pattern.Match(text);
                return match.Success ? () => action(match) : (Action)null;
            };

            var patterns = new[]
            {
                matchDo(_unorderedListPattern, m => document.Insert(_editor.SelectionStart, m.Groups[0].Value.TrimStart() + " ")),
                matchDo(_unorderedListEndPattern, m => document.Remove(line)),
                matchDo(_orderedListPattern, m =>
                {
                    var number = int.Parse(m.Groups[1].Value) + 1;
                    document.Insert(_editor.SelectionStart, number + ". ");
                    RenumberOrderedList(document, line.NextLine, text, number);
                }),
                matchDo(_orderedListEndPattern, m => document.Remove(line)),
                matchDo(_blockQuotePattern, m => document.Insert(_editor.SelectionStart, m.Groups[1].Value.TrimStart())),
                matchDo(_blockQuoteEndPattern, m => document.Remove(line))
            };

            patterns.FirstOrDefault(action => action != null)?.Invoke();
        }

        private void RenumberOrderedList(
            ICSharpCode.AvalonEdit.Document.TextDocument document, 
            ICSharpCode.AvalonEdit.Document.DocumentLine line, 
            string text, 
            int number)
        {
            while ((line = line.NextLine) != null)
            {
                number += 1;
                text = document.GetText(line.Offset, line.Length);
                var match = _orderedListPattern.Match(text);
                if (match.Success == false) break;
                var group = match.Groups[1];
                var num = int.Parse(group.Value);
                if (num != number) document.Replace(line.Offset + group.Index, group.Length, number.ToString());
            }
        }
    }
}