using System;
using System.Linq;
using ICSharpCode.AvalonEdit;
using Infrastructure;

namespace ServicesModule
{
    public class EditService : IEditService
    {
        private static bool IsWordPart(char ch) => char.IsLetterOrDigit(ch) || ch == '_' || ch == '*';

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
    }
}
