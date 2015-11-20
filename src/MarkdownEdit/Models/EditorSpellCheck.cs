using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Document;
using MarkdownEdit.Controls;
using MarkdownEdit.i18n;

namespace MarkdownEdit.Models
{
    public static class EditorSpellCheck
    {
        public static bool AppsKeyDown;

        public static void SpellCheckSuggestions(Editor editor, ContextMenu contextMenu)
        {
            if (editor.SpellCheckProvider != null)
            {
                int offset;
                if (AppsKeyDown || IsAlternateAppsKeyShortcut)
                {
                    AppsKeyDown = false;
                    offset = editor.EditBox.SelectionStart;
                }
                else
                {
                    var editorPosition = editor.EditBox.GetPositionFromPoint(Mouse.GetPosition(editor.EditBox));
                    if (!editorPosition.HasValue) return;
                    offset = editor.EditBox.Document.GetOffset(editorPosition.Value.Line, editorPosition.Value.Column);
                }

                var errorSegments = editor.SpellCheckProvider.GetSpellCheckErrors();
                var misspelledSegment = errorSegments.FirstOrDefault(segment => segment.StartOffset <= offset && segment.EndOffset >= offset);
                if (misspelledSegment == null) return;

                // check if the clicked offset is the beginning or end of line to prevent snapping to it
                // (like in text selection) with GetPositionFromPoint
                // in practice makes context menu not show when clicking on the first character of a line
                var currentLine = editor.EditBox.Document.GetLineByOffset(offset);
                if (offset == currentLine.Offset || offset == currentLine.EndOffset) return;

                var misspelledText = editor.EditBox.Document.GetText(misspelledSegment);
                var suggestions = editor.SpellCheckProvider.GetSpellCheckSuggestions(misspelledText);
                foreach (var item in suggestions) contextMenu.Items.Add(SpellSuggestMenuItem(item, misspelledSegment));
                contextMenu.Items.Add(new MenuItem
                {
                    Header = TranslationProvider.Translate("editor-menu-add-to-dictionary"),
                    Command = EditingCommands.IgnoreSpellingError,
                    CommandParameter = misspelledText
                });
                contextMenu.Items.Add(new Separator());
            }
        }

        private static MenuItem SpellSuggestMenuItem(string header, TextSegment segment)
        {
            return new MenuItem
            {
                Header = header,
                FontWeight = FontWeights.Bold,
                Command = EditingCommands.CorrectSpellingError,
                CommandParameter = new Tuple<string, TextSegment>(header, segment)
            };
        }

        private static bool IsAlternateAppsKeyShortcut =>
            Keyboard.IsKeyDown(Key.F10) && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift));
    }
}