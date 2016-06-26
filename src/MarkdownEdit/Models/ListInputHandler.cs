using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using PM = System.Tuple<System.Text.RegularExpressions.Regex, System.Action<System.Text.RegularExpressions.Match>>;

namespace MarkdownEdit.Models
{
    // When user adds or removes items from an ordered/unorderd list, adjust by renumbering,
    // or adding a new item depending on the user action.

    public static class ListInputHandler
    {
        public static readonly Regex UnorderedListCheckboxPattern = new Regex(@"^[ ]{0,3}[-\*\+][ ]{1,3}\[[ xX]\](?=[ ]{1,3}\S)", RegexOptions.Compiled);
        public static readonly Regex UnorderedListCheckboxEndPattern = new Regex(@"^[ ]{0,3}[-\*\+][ ]{1,3}\[[ xX]\]\s*", RegexOptions.Compiled);
        public static readonly Regex OrderedListPattern = new Regex(@"^[ ]{0,3}(\d+)\.(?=[ ]{1,3}\S)", RegexOptions.Compiled);
        public static readonly Regex OrderedListEndPattern = new Regex(@"^[ ]{0,3}(\d+)\.(?=[ ]{1,3}\s*)", RegexOptions.Compiled);
        public static readonly Regex UnorderedListPattern = new Regex(@"^[ ]{0,3}[-\*\+](?=[ ]{1,3}\S)", RegexOptions.Compiled);
        public static readonly Regex UnorderedListEndPattern = new Regex(@"^[ ]{0,3}[-\*\+](?=[ ]{1,3}\s*)", RegexOptions.Compiled);
        public static readonly Regex BlockQuotePattern = new Regex(@"^(([ ]{0,4}>)+)[ ]{0,4}.{2}", RegexOptions.Compiled);
        public static readonly Regex BlockQuoteEndPattern = new Regex(@"^([ ]{0,4}>)+[ ]{0,4}\s*", RegexOptions.Compiled);

        public static void AdjustList(TextEditor editor, bool lineCountIncreased)
        {
            var document = editor.Document;
            var line = document.GetLineByOffset(editor.SelectionStart)?.PreviousLine;
            if (line == null) return;
            var text = document.GetText(line.Offset, line.Length);

            // A poor mans pattern matcher

            Action<string, IEnumerable<PM>> match = (txt, patterns) =>
            {
                var firstMatch = patterns
                    .Select(pattern => new {Match = pattern.Item1.Match(txt), Action = pattern.Item2})
                    .FirstOrDefault(ma => ma.Match.Success);

                firstMatch?.Action(firstMatch.Match);
            };

            document.BeginUpdate();

            match(text, new[]
            {
                new PM(UnorderedListCheckboxPattern, m => document.Insert(editor.SelectionStart, m.Groups[0].Value.TrimStart() + " ")),
                new PM(UnorderedListCheckboxEndPattern, m => document.Remove(line)),
                new PM(UnorderedListPattern, m => document.Insert(editor.SelectionStart, m.Groups[0].Value.TrimStart() + " ")),
                new PM(UnorderedListEndPattern, m => document.Remove(line)),
                new PM(OrderedListPattern, m =>
                {
                    var number = int.Parse(m.Groups[1].Value);
                    if (lineCountIncreased)
                    {
                        number += 1;
                        document.Insert(editor.SelectionStart, number + ". ");
                        line = line.NextLine;
                    }
                    RenumberOrderedList(document, line, number);
                }),
                new PM(OrderedListEndPattern, m => document.Remove(line)),
                new PM(BlockQuotePattern, m => document.Insert(editor.SelectionStart, m.Groups[1].Value.TrimStart())),
                new PM(BlockQuoteEndPattern, m => document.Remove(line))
            });

            document.EndUpdate();
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