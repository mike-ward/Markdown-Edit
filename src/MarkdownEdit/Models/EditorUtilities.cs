using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit;

namespace MarkdownEdit.Models
{
    public static class EditorUtilities
    {
        public static void SelectWordAt(this TextEditor editor, int offset)
        {
            if (offset < 0 || offset >= editor.Document.TextLength || !IsWordPart(editor.Document.GetCharAt(offset)))
            {
                return;
            }

            var startOffset = offset;
            var endOffset = offset;
            while (startOffset > 0 && IsWordPart(editor.Document.GetCharAt(startOffset - 1))) startOffset -= 1;
            while (endOffset < editor.Document.TextLength - 1 && IsWordPart(editor.Document.GetCharAt(endOffset + 1))) endOffset += 1;
            editor.Select(startOffset, endOffset - startOffset + 1);
        }

        private static bool IsWordPart(char ch)
        {
            return char.IsLetterOrDigit(ch) || ch == '_' || ch == '*';
        }

        public static void SelectHeader(this TextEditor editor, bool next)
        {
            var start = editor.SelectionStart + (next ? editor.SelectionLength : 0);
            var options = RegexOptions.Multiline | (next ? RegexOptions.None : RegexOptions.RightToLeft);
            var regex = new Regex("^#{1,6}[^#].*", options);
            var match = regex.Match(editor.Text, start);
            if (!match.Success)
            {
                Utility.Beep();
                return;
            }
            editor.Select(match.Index, match.Length);
            var loc = editor.Document.GetLocation(match.Index);
            editor.ScrollTo(loc.Line, loc.Column);
        }

        public static void AddRemoveText(this TextEditor editor, string quote)
        {
            var selected = editor.SelectedText;

            if (string.IsNullOrEmpty(selected))
            {
                editor.SelectWordAt(editor.CaretOffset);
                selected = editor.SelectedText;
            }

            if (string.IsNullOrEmpty(selected))
            {
                editor.Document.Insert(editor.TextArea.Caret.Offset, quote);
            }
            else
            {
                editor.SelectedText = (selected.StartsWith(quote) && selected.EndsWith(quote))
                    ? selected.UnsurroundWith(quote)
                    : selected.SurroundWith(quote);
            }
        }

        public static bool Find(this TextEditor editor, Regex find)
        {
            try
            {
                var previous = find.Options.HasFlag(RegexOptions.RightToLeft);

                var start = previous
                    ? editor.SelectionStart
                    : editor.SelectionStart + editor.SelectionLength;

                var match = find.Match(editor.Text, start);
                if (!match.Success) // start again from beginning or end
                {
                    match = find.Match(editor.Text, previous ? editor.Text.Length : 0);
                }

                if (match.Success)
                {
                    editor.Select(match.Index, match.Length);
                    var loc = editor.Document.GetLocation(match.Index);
                    editor.ScrollTo(loc.Line, loc.Column);
                }

                return match.Success;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
                return false;
            }
        }

        public static bool Replace(this TextEditor editor, Regex find, string replace)
        {
            try
            {
                var input = editor.Text.Substring(editor.SelectionStart, editor.SelectionLength);
                var match = find.Match(input);
                var replaced = false;
                if (match.Success && match.Index == 0 && match.Length == input.Length)
                {
                    var replaceWith = match.Result(replace);
                    editor.Document.Replace(editor.SelectionStart, editor.SelectionLength, replaceWith);
                    replaced = true;
                }

                if (!editor.Find(find) && !replaced) Utility.Beep();
                return replaced;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
                return false;
            }
        }

        public static void ReplaceAll(this TextEditor editor, Regex find, string replace)
        {
            try
            {
                var offset = 0;
                editor.BeginChange();
                foreach (Match match in find.Matches(editor.Text))
                {
                    var replaceWith = match.Result(replace);
                    editor.Document.Replace(offset + match.Index, match.Length, replaceWith);
                    offset += replaceWith.Length - match.Length;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                editor.EndChange();
            }
        }

        public static bool ErrorBeep()
        {
            Utility.Beep();
            return false;
        }
    }
}