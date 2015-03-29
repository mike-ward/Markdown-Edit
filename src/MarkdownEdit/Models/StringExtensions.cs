using System;
using System.Text.RegularExpressions;

namespace MarkdownEdit.Models
{
    internal static class StringExtensions
    {
        public static string SurroundWith(this string text, string quote)
        {
            return $"{quote}{text}{quote}";
        }

        public static string UnsurroundWith(this string text, string quote)
        {
            return text.Trim(quote.ToCharArray());
        }

        public static string ReplaceSmartChars(this string smart)
        {
            var dumb = smart
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
            return dumb;
        }

        public static int WordCount(this string text)
        {
            return (text == null) ? 0 : Regex.Matches(text, @"[\S]+").Count;
        }

        public static string ReplaceDate(this string text)
        {
            var datePattern = new Regex(@"\$DATE(?:\(""(.+)""\))?\$");
            return datePattern.Replace(text, match => DateTime.Now.ToString(match?.Groups[1].Value));
        }

        public static string AddOffsetToFileName(this string file, int offset)
        {
            return $"{file.StripOffsetFromFileName()}|{offset}"; 
        }

        public static string StripOffsetFromFileName(this string file)
        {
            if (string.IsNullOrWhiteSpace(file)) return file;
            var index = file.IndexOf('|');
            return (index >= 0) ? file.Substring(0, index) : file;
        }
    }
}