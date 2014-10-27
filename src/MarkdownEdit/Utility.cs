using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MarkdownEdit
{
    internal static class Utility
    {
        public static Func<TKey, TResult> Memoize<TKey, TResult>(this Func<TKey, TResult> func)
        {
            var cache = new ConcurrentDictionary<TKey, TResult>();
            return key => cache.GetOrAdd(key, func);
        }

        public static Action<T> Debounce<T>(this Action<T> func, int milliseconds = 300)
        {
            // ReSharper disable once TooWideLocalVariableScope
            T last;
            return arg =>
            {
                last = arg;
                Task.Delay(milliseconds).ContinueWith(t =>
                {
                    if (last.Equals(arg)) func(last);
                    t.Dispose();
                });
            };
        }

        public static void Beep()
        {
            SystemSounds.Beep.Play();
        }

        public static string WrapToColumn(string text, int column = 90)
        {
            var allParagraphs = new List<string>();
            var separators = new [] { "\n\n", "\r\r", "\r\n\r\n" };
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