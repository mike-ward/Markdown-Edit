using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;

namespace MarkdownEdit.Commands
{
    public class LineContinuationEnterCommand : ICommand
    {
        private readonly TextEditor _editor;
        private readonly ICommand _baseCommand;

        public static readonly Regex OrderedListPattern = new Regex(@"^[ ]{0,3}(\d+)\.(?=[ ]{1,3}\S)", RegexOptions.Compiled);
        public static readonly Regex OrderedListEndPattern = new Regex(@"^[ ]{0,3}(\d+)\.(?=[ ]{1,3}\s*)", RegexOptions.Compiled);
        public static readonly Regex UnorderedListPattern = new Regex(@"^[ ]{0,3}[-\*\+](?=[ ]{1,3}\S)", RegexOptions.Compiled);
        public static readonly Regex UnorderedListEndPattern = new Regex(@"^[ ]{0,3}[-\*\+](?=[ ]{1,3}\s*)", RegexOptions.Compiled);
        public static readonly Regex BlockQuotePattern = new Regex(@"^(([ ]{0,4}>)+)[ ]{0,4}.{2}", RegexOptions.Compiled);
        public static readonly Regex BlockQuoteEndPattern = new Regex(@"^([ ]{0,4}>)+[ ]{0,4}\s*", RegexOptions.Compiled);

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
                matchDo(UnorderedListPattern, m => document.Insert(_editor.SelectionStart, m.Groups[0].Value.TrimStart() + " ")),
                matchDo(UnorderedListEndPattern, m => document.Remove(line)),
                matchDo(OrderedListPattern, m =>
                {
                    var number = int.Parse(m.Groups[1].Value) + 1;
                    document.Insert(_editor.SelectionStart, number + ". ");
                    RenumberOrderedList(document, line.NextLine, number);
                }),
                matchDo(OrderedListEndPattern, m => document.Remove(line)),
                matchDo(BlockQuotePattern, m => document.Insert(_editor.SelectionStart, m.Groups[1].Value.TrimStart())),
                matchDo(BlockQuoteEndPattern, m => document.Remove(line))
            };

            patterns.FirstOrDefault(action => action != null)?.Invoke();
        }

        private static void RenumberOrderedList(
            IDocument document, 
            DocumentLine line, 
            int number)
        {
            while ((line = line.NextLine) != null)
            {
                number += 1;
                var text = document.GetText(line.Offset, line.Length);
                var match = OrderedListPattern.Match(text);
                if (match.Success == false) break;
                var group = match.Groups[1];
                var currentNumber = int.Parse(group.Value);
                if (currentNumber != number) document.Replace(line.Offset + group.Index, group.Length, number.ToString());
            }
        }
    }
}