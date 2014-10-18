using System.Windows.Markup;

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
    }
}