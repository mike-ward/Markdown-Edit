using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MarkdownEdit
{
    internal static class StringExtensions
    {
        public static string SurroundWith(this string text, string quote)
        {
            return string.Format("{1}{0}{1}", text, quote);
        }

        public static string UnsurroundWith(this string text, string quote)
        {
            if (text.StartsWith(quote)) text = text.Remove(0, quote.Length);
            if (text.EndsWith(quote)) text = text.Remove(text.Length - quote.Length);
            return text;
        }

        public static string ReplaceSmartChars(this string text)
        {
            return text
                .Replace('\u2013', '-')
                .Replace('\u2014', '-')
                .Replace('\u2015', '-')
                .Replace('\u2017', '_')
                .Replace('\u2018', '\'')
                .Replace('\u2019', '\'')
                .Replace('\u201a', ',')
                .Replace('\u201b', '\'')
                .Replace('\u201c', '\"')
                .Replace('\u201d', '\"')
                .Replace('\u201e', '\"')
                .Replace("\u2026", "...")
                .Replace('\u2032', '\'')
                .Replace('\u2033', '\"');
        }

        public static string WrapToColumn(this string text, int column = 90)
        {
            var allParagraphs = new List<string>();
            var separators = new[] {"\n\n", "\r\r", "\r\n\r\n"};
            var paragraphs = text.Split(separators, StringSplitOptions.RemoveEmptyEntries).Where(s => !string.IsNullOrEmpty(s)).ToArray();
            foreach (var paragraph in paragraphs)
            {
                if (Regex.IsMatch(paragraph, @"^(\s{4,}|\t)"))
                {
                    allParagraphs.Add(paragraph);
                    continue;
                }
                var lines = new List<string>();
                var line = new List<string>();
                var words = paragraph.Split(null);
                foreach (var word in words)
                {
                    var ltext = string.Join(" ", line);
                    if (ltext.Length + word.Length + 1 > column)
                    {
                        lines.Add(ltext);
                        line.Clear();
                    }
                    line.Add(word);
                }
                if (line.Count > 0) lines.Add(string.Join(" ", line));
                allParagraphs.Add(string.Join("\n", lines));
            }
            return string.Join("\r\n\r\n", allParagraphs);
        }
    }
}