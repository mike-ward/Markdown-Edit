using System;
using System.Text;
using System.Text.RegularExpressions;

namespace MarkdownEdit.Models
{
    internal static class StringExtensions
    {
        public static string SurroundWith(this string text, string quote) => $"{quote}{text}{quote}";

        public static string UnsurroundWith(this string text, string quote) => text.Trim(quote.ToCharArray());

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

        public static int WordCount(this string text) => text == null ? 0 : Regex.Matches(text, @"[\S]+").Count;

        public static string AddOffsetToFileName(this string file, int offset)
            => $"{file.StripOffsetFromFileName()}|{offset}";

        public static string ReplaceDate(this string text)
        {
            var datePattern = new Regex(@"\$DATE(?:\(""(.+)""\))?\$");
            return datePattern.Replace(text, match => DateTime.Now.ToString(match?.Groups[1].Value));
        }

        public static string StripOffsetFromFileName(this string file)
        {
            if (string.IsNullOrWhiteSpace(file)) return file;
            var index = file.IndexOf('|');
            return index >= 0 ? file.Substring(0, index) : file;
        }

        public static string ToSlug(this string value, bool toLower = false)
        {
            if (value == null) return "";
            var normalised = value.Normalize(NormalizationForm.FormKD);

            const int maxlen = 80;
            var len = normalised.Length;
            var prevDash = false;
            var sb = new StringBuilder(len);

            for (var i = 0; i < len; i++)
            {
                var c = normalised[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    if (prevDash)
                    {
                        sb.Append('-');
                        prevDash = false;
                    }
                    sb.Append(c);
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    if (prevDash)
                    {
                        sb.Append('-');
                        prevDash = false;
                    }
                    // tricky way to convert to lowercase
                    if (toLower) sb.Append((char)(c | 32));
                    else sb.Append(c);
                }
                else if (c == ' ' || c == ',' || c == '.' || c == '/' || c == '\\' || c == '-' || c == '_' || c == '=')
                {
                    if (!prevDash && sb.Length > 0)
                    {
                        prevDash = true;
                    }
                }
                else
                {
                    var swap = ConvertEdgeCases(c, toLower);

                    if (swap != null)
                    {
                        if (prevDash)
                        {
                            sb.Append('-');
                            prevDash = false;
                        }
                        sb.Append(swap);
                    }
                }

                if (sb.Length == maxlen) break;
            }

            return sb.ToString();
        }

        private static string ConvertEdgeCases(char c, bool toLower)
        {
            string swap = null;
            switch (c)
            {
                case 'ı':
                    swap = "i";
                    break;

                case 'ł':
                    swap = "l";
                    break;

                case 'Ł':
                    swap = toLower ? "l" : "L";
                    break;

                case 'đ':
                    swap = "d";
                    break;

                case 'ß':
                    swap = "ss";
                    break;

                case 'ø':
                    swap = "o";
                    break;

                case 'Þ':
                    swap = "th";
                    break;
            }
            return swap;
        }
    }
}