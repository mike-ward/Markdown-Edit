using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;

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

        public static void MoveSegmentUp(TextEditor textEditor)
        {
            var textArea = textEditor.TextArea;
            var document = textEditor.Document;

            int currentLine;
            DocumentLine segment;
            var selection = textArea.Selection;
            if (selection != null && selection.StartPosition.Line > 1)
            {
                currentLine = Math.Max(selection.StartPosition.Line, selection.EndPosition.Line);
                segment = document.GetLineByNumber(Math.Min(selection.StartPosition.Line, selection.EndPosition.Line)).PreviousLine;
            }
            else
            {
                currentLine = textArea.Caret.Line;
                if (currentLine == 1) return;
                segment = document.GetLineByNumber(currentLine).PreviousLine;
            }

            textEditor.BeginChange();
            try
            {
                var text = document.GetText(segment.Offset, segment.TotalLength);
                document.Remove(segment.Offset, segment.TotalLength);
                var offset = currentLine >= document.LineCount
                    ? document.TextLength
                    : document.GetOffset(currentLine, 0);
                document.Insert(offset, text);
                textArea.Caret.Line = currentLine - 1;
                textArea.Caret.BringCaretToView();
            }
            finally
            {
                textEditor.EndChange();
                textArea.TextView.Redraw(DispatcherPriority.ApplicationIdle);
            }
        }

        public static void MoveSegmentDown(TextEditor textEditor)
        {
            var textArea = textEditor.TextArea;
            var document = textEditor.Document;

            int currentLine;
            DocumentLine segment;
            var selection = textArea.Selection;
            if (selection != null && selection.StartPosition.Line > 0 && selection.EndPosition.Line < textEditor.LineCount)
            {
                currentLine = Math.Min(selection.StartPosition.Line, selection.EndPosition.Line);
                segment = document.GetLineByNumber(Math.Max(selection.StartPosition.Line, selection.EndPosition.Line)).NextLine;
            }
            else
            {
                currentLine = textArea.Caret.Line;
                if (currentLine >= textEditor.LineCount) return;
                segment = document.GetLineByNumber(currentLine).NextLine;
            }

            textEditor.BeginChange();
            try
            {
                var text = document.GetText(segment.Offset, segment.TotalLength);
                document.Remove(segment.Offset, segment.TotalLength);
                document.Insert(document.GetOffset(currentLine, 0), text);
                textArea.Caret.Line = currentLine + 1;
                textArea.Caret.BringCaretToView();
            }
            finally
            {
                textEditor.EndChange();
                textArea.TextView.Redraw(DispatcherPriority.ApplicationIdle);
            }
        }

        public static void ConvertSelectionToList(TextEditor textEditor)
        {
            var selection = textEditor.TextArea.Selection;
            var start = Math.Min(selection.StartPosition.Line, selection.EndPosition.Line);
            if (start == 0) return;
            var end = Math.Max(selection.StartPosition.Line, selection.EndPosition.Line);
            var document = textEditor.Document;
            var ordered = new Regex(@"^\s*\d+\.\s{1,4}");
            var unordered = new Regex(@"^\s*[-|\*|\+]\s{1,4}");

            textEditor.BeginChange();
            var index = 1;
            try
            {
                foreach (var num in Enumerable.Range(start, end - start + 1))
                {
                    var line = document.GetLineByNumber(num);
                    var offset = line.Offset;
                    var text = document.GetText(line);
                    if (string.IsNullOrWhiteSpace(text)) continue;

                    if (unordered.IsMatch(text))
                    {
                        var numbered = Regex.Replace(text, @"[-|\*|\+]", $"{index++}.");
                        document.Remove(line);
                        document.Insert(offset, numbered);
                    }
                    else if (ordered.IsMatch(text))
                    {
                        var unnumbered = Regex.Replace(text, @"\d+\.", "-");
                        document.Remove(line);
                        document.Insert(offset, unnumbered);
                    }
                    else
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

        public static void InsertBlockQuote(TextEditor textEditor)
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

        public static void InsertHyperlink(TextEditor editor, string link)
        {
            if (string.IsNullOrWhiteSpace(link)) return;

            var parts = link.Split(new[] {'"'}, 2);
            var text = parts.Length == 1
                ? $"<{parts[0].Trim()}>"
                : $"[{parts[1].Trim('"', ' ')}]({parts[0].Trim()})";

            editor.Document.Replace(editor.SelectionStart, editor.SelectionLength, text);
        }
    }
}