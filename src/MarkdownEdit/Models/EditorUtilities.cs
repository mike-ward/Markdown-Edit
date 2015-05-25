using System;
using System.Diagnostics;
using System.Linq;
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
            var document = editor.Document;
            while (startOffset > 0 && IsWordPart(document.GetCharAt(startOffset - 1))) startOffset -= 1;
            while (endOffset < document.TextLength - 1 && IsWordPart(document.GetCharAt(endOffset + 1))) endOffset += 1;
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

        public static void MoveCurrentLineUp(TextEditor textEditor)
        {
            var textArea = textEditor.TextArea;
            var document = textEditor.Document;
            var line = textArea.Caret.Line;
            if (line == 1) return;

            var documentLine = document.GetLineByNumber(line);
            var documentLinePrevious = documentLine.PreviousLine;

            var text = textArea.Document.GetText(documentLine);
            var previousText = textArea.Document.GetText(documentLinePrevious);

            textEditor.BeginChange();
            try
            {
                document.Remove(documentLine);
                document.Remove(documentLinePrevious);

                document.Insert(documentLinePrevious.Offset, text);
                document.Insert(documentLine.Offset, previousText);

                textArea.Caret.Line = line - 1;
                textArea.Caret.BringCaretToView();
            }
            finally
            {
                textEditor.EndChange();
            }
        }

        public static void MoveCurrentLineDown(TextEditor textEditor)
        {
            var textArea = textEditor.TextArea;
            var document = textEditor.Document;
            var line = textArea.Caret.Line;
            if (line == textEditor.LineCount) return;

            var documentLine = document.GetLineByNumber(line);
            var documentLineNext = documentLine.NextLine;

            var text = textArea.Document.GetText(documentLine);
            var nextText = textArea.Document.GetText(documentLineNext);

            textEditor.BeginChange();
            try
            {
                document.Remove(documentLine);
                document.Remove(documentLineNext);

                document.Insert(documentLineNext.Offset, text);
                document.Insert(documentLine.Offset, nextText);

                textArea.Caret.Line = line + 1;
                textArea.Caret.BringCaretToView();
            }
            finally
            {
                textEditor.EndChange();
            }
        }

        public static void ConvertSelectionToList(TextEditor textEditor)
        {
            var selection = textEditor.TextArea.Selection;
            var start = selection.StartPosition.Line;
            if (start == 0) return;
            var end = selection.EndPosition.Line;
            var document = textEditor.Document;
            var startsWith = new Regex(@"\s*[-|\*|\+]\s{1,4}");

            textEditor.BeginChange();
            try
            {
                foreach (var num in Enumerable.Range(start, end - start + 1))
                {
                    var line = document.GetLineByNumber(num);
                    var text = document.GetText(line);
                    if (string.IsNullOrWhiteSpace(text) == false && startsWith.IsMatch(text) == false)
                    {
                        var indexOfFirstChar = text.TakeWhile(char.IsWhiteSpace).Count();
                        document.Insert(line.Offset + indexOfFirstChar, "- ");
                    }
                }
            }
            finally
            {
                textEditor.EndChange();
            }
        }

        public static void ScrollToLine(TextEditor editor, int line)
        {
            var max = Math.Max(1, editor.Document.LineCount);
            line = Math.Min(max, Math.Max(line, 1));
            editor.ScrollToLine(line);
            var offset = editor.Document.GetOffset(line, 0);
            editor.CaretOffset = offset;
        }
    }
}