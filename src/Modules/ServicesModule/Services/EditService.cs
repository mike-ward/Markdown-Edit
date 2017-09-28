using System;
using System.Linq;
using System.Media;
using System.Text.RegularExpressions;
using System.Windows;
using ICSharpCode.AvalonEdit;
using Infrastructure;

namespace ServicesModule.Services
{
    public class EditService : IEditService
    {
        private readonly INotify _notify;

        public EditService(INotify notify)
        {
            _notify = notify;
        }

        private static bool IsWordPart(char ch) => char.IsLetterOrDigit(ch) || ch == '_' || ch == '*';

        public FindReplaceOptions FindReplaceOptions { get; } = new FindReplaceOptions();

        public void SelectWordAt(TextEditor editor, int offset)
        {
            if (offset < 0 || offset >= editor.Document.TextLength || !IsWordPart(editor.Document.GetCharAt(offset)))
            {
                return;
            }

            var startOffset = offset;
            var endOffset = offset;
            var document = editor.Document;
            while (startOffset > 0 && IsWordPart(document.GetCharAt(startOffset - 1))) startOffset -= 1;
            while (endOffset < document.TextLength - 1 && IsWordPart(document.GetCharAt(endOffset + 1))) endOffset += 1;
            editor.Select(startOffset, endOffset - startOffset + 1);
        }

        public void AddRemoveText(TextEditor editor, string quote)
        {
            var selected = editor.SelectedText;

            if (string.IsNullOrEmpty(selected))
            {
                SelectWordAt(editor, editor.CaretOffset);
                selected = editor.SelectedText;
            }

            if (string.IsNullOrEmpty(selected))
            {
                editor.Document.Insert(editor.TextArea.Caret.Offset, quote);
            }
            else
            {
                editor.SelectedText = selected.StartsWith(quote) && selected.EndsWith(quote)
                    ? selected.Remove(selected.Length - 1).Remove(0, 1)
                    : $"{quote}{selected}{quote}";
            }
        }

        public void InsertBlockQuote(TextEditor textEditor)
        {
            var textArea = textEditor.TextArea;
            var document = textEditor.Document;
            var selection = textEditor.TextArea.Selection;

            var start = Math.Min(selection.StartPosition.Line, selection.EndPosition.Line);
            if (start == 0) start = textArea.Caret.Line;
            var end = Math.Max(selection.StartPosition.Line, selection.EndPosition.Line);
            if (end == 0) end = textArea.Caret.Line;

            textEditor.BeginChange();
            try
            {
                foreach (var line in Enumerable.Range(start, end - start + 1))
                {
                    var offset = document.GetOffset(line, 0);
                    document.Insert(offset, "> ");
                }
            }
            finally
            {
                textEditor.EndChange();
            }
        }

        public bool FindNext(TextEditor textEditor, FindReplaceOptions findReplaceOptions)
        {

            var regex = GetRegEx(findReplaceOptions);

            var start = regex.Options.HasFlag(RegexOptions.RightToLeft)
                ? textEditor.SelectionStart
                : textEditor.SelectionStart + textEditor.SelectionLength;

            var match = regex.Match(textEditor.Text, start);

            if (!match.Success)  // start again from beginning or end
            {
                match = regex.Match(textEditor.Text, regex.Options.HasFlag(RegexOptions.RightToLeft)
                    ? textEditor.Text.Length
                    : 0);
            }

            if (match.Success)
            {
                textEditor.Select(match.Index, match.Length);
                var loc = textEditor.Document.GetLocation(match.Index);
                textEditor.ScrollTo(loc.Line, loc.Column);
            }

            return match.Success;
        }

        public void Replace(TextEditor textEditor, FindReplaceOptions findReplaceOptions)
        {
            var regex = GetRegEx(findReplaceOptions);
            var input = textEditor.Text.Substring(textEditor.SelectionStart, textEditor.SelectionLength);
            var match = regex.Match(input);
            var replaced = false;
            if (match.Success && match.Index == 0 && match.Length == input.Length)
            {
                textEditor.Document.Replace(textEditor.SelectionStart, textEditor.SelectionLength, findReplaceOptions.ReplaceText);
                replaced = true;
            }

            if (!FindNext(textEditor, findReplaceOptions) && !replaced) SystemSounds.Beep.Play();
        }

        public void ReplaceAll(TextEditor editor, FindReplaceOptions findReplaceOptions)
        {
            if(_notify.ConfirmYesNo(
                  $"Are you sure you want to Replace All occurences of \"{findReplaceOptions.FindText}" +
                  $"\" with \"{findReplaceOptions.ReplaceText}\"?") == MessageBoxResult.Yes)
            {
                var regex = GetRegEx(findReplaceOptions);
                var offset = 0;
                editor.BeginChange();

                foreach (Match match in regex.Matches(editor.Text))
                {
                    editor.Document.Replace(offset + match.Index, match.Length, findReplaceOptions.FindText);
                    offset += findReplaceOptions.ReplaceText.Length - match.Length;
                }

                editor.EndChange();
            }
        }

        private Regex GetRegEx(FindReplaceOptions findReplaceOptions)
        {
            var options = RegexOptions.None;
            if (findReplaceOptions.SearchUp && findReplaceOptions.RightToLeft) options |= RegexOptions.RightToLeft;
            if (findReplaceOptions.CaseSensitive == false) options |= RegexOptions.IgnoreCase;
            if (findReplaceOptions.Regex) return new Regex(findReplaceOptions.FindText, options);

            var pattern = Regex.Escape(findReplaceOptions.FindText);
            if (findReplaceOptions.Wildcards) pattern = pattern.Replace("\\*", ".*").Replace("\\?", ".");
            if (findReplaceOptions.WholeWord) pattern = "\\b" + pattern + "\\b";
            return new Regex(pattern, options);
        }
    }
}
