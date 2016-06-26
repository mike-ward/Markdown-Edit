using System;
using System.Linq;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;

namespace MarkdownEdit.Models
{
    // When user adds or removes items from an ordered/unorderd list, adjust by renumbering,
    // or adding a new item depending on the user action.

    public static class ListInputHandler
    {
        public static readonly Regex UnorderedListCheckboxPattern =
            new Regex(@"^[ ]{0,3}[-\*\+][ ]{1,3}\[[ xX]\](?=[ ]{1,3}\S)", RegexOptions.Compiled);

        public static readonly Regex UnorderedListCheckboxEndPattern = new Regex(
            @"^[ ]{0,3}[-\*\+][ ]{1,3}\[[ xX]\]\s*", RegexOptions.Compiled);

        public static readonly Regex OrderedListPattern = new Regex(@"^[ ]{0,3}(\d+)\.(?=[ ]{1,3}\S)",
            RegexOptions.Compiled);

        public static readonly Regex OrderedListEndPattern = new Regex(@"^[ ]{0,3}(\d+)\.(?=[ ]{1,3}\s*)",
            RegexOptions.Compiled);

        public static readonly Regex UnorderedListPattern = new Regex(@"^[ ]{0,3}[-\*\+](?=[ ]{1,3}\S)",
            RegexOptions.Compiled);

        public static readonly Regex UnorderedListEndPattern = new Regex(@"^[ ]{0,3}[-\*\+](?=[ ]{1,3}\s*)",
            RegexOptions.Compiled);

        public static readonly Regex BlockQuotePattern = new Regex(@"^(([ ]{0,4}>)+)[ ]{0,4}.{2}", RegexOptions.Compiled);
        public static readonly Regex BlockQuoteEndPattern = new Regex(@"^([ ]{0,4}>)+[ ]{0,4}\s*", RegexOptions.Compiled);

        public static void AdjustList(TextEditor editor, bool lineCountGrowing)
        {
            var document = editor.Document;
            var line = document.GetLineByOffset(editor.SelectionStart)?.PreviousLine;
            if (line == null) return;
            var text = document.GetText(line.Offset, line.Length);

            Func<Regex, Action<Match>, Action> matchDo = (pattern, action) =>
            {
                var match = pattern.Match(text);
                return match.Success ? () => action(match) : (Action)null;
            };

            var patterns = new[]
            {
                matchDo(UnorderedListCheckboxPattern,
                    m => document.Insert(editor.SelectionStart, m.Groups[0].Value.TrimStart() + " ")),
                matchDo(UnorderedListCheckboxEndPattern, m => document.Remove(line)),
                matchDo(UnorderedListPattern,
                    m => document.Insert(editor.SelectionStart, m.Groups[0].Value.TrimStart() + " ")),
                matchDo(UnorderedListEndPattern, m => document.Remove(line)),
                matchDo(OrderedListPattern, m =>
                {
                    var number = int.Parse(m.Groups[1].Value);
                    if (lineCountGrowing)
                    {
                        number += 1;
                        document.Insert(editor.SelectionStart, number + ". ");
                        line = line.NextLine;
                    }
                    RenumberOrderedList(document, line, number);
                }),
                matchDo(OrderedListEndPattern, m => document.Remove(line)),
                matchDo(BlockQuotePattern, m => document.Insert(editor.SelectionStart, m.Groups[1].Value.TrimStart())),
                matchDo(BlockQuoteEndPattern, m => document.Remove(line))
            };

            patterns.FirstOrDefault(action => action != null)?.Invoke();
        }

        private static void RenumberOrderedList(
            IDocument document,
            DocumentLine line,
            int number)
        {
            if (line == null) return;
            while ((line = line.NextLine) != null)
            {
                number += 1;
                var text = document.GetText(line.Offset, line.Length);
                var match = OrderedListPattern.Match(text);
                if (match.Success == false) break;
                var group = match.Groups[1];
                var currentNumber = int.Parse(group.Value);
                if (currentNumber != number)
                {
                    document.Replace(line.Offset + group.Index, group.Length, number.ToString());
                }
            }
        }
    }
}