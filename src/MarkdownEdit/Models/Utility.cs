using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using MarkdownEdit.MarkdownConverters;

namespace MarkdownEdit.Models
{
    public static class Utility
    {
        public static Func<TKey, TResult> Memoize<TKey, TResult>(this Func<TKey, TResult> func)
        {
            var cache = new ConcurrentDictionary<TKey, TResult>();
            return key => cache.GetOrAdd(key, func);
        }

        public static Action Debounce(this Action func, int milliseconds = 300)
        {
            var action = Debounce<int>(_ => func(), milliseconds);
            return () => action(0);
        }

        public static Action<T> Debounce<T>(this Action<T> func, int milliseconds = 300)
        {
            var last = long.MinValue;
            return arg =>
            {
                try
                {
                    var current = Interlocked.Increment(ref last);
                    Task.Delay(milliseconds).ContinueWith(task =>
                    {
                        // ReSharper disable once AccessToModifiedClosure
                        if (current == last) func(arg);
                        task.Dispose();
                    });
                }
                catch (OverflowException)
                {
                    Interlocked.Exchange(ref last, long.MinValue);
                }
            };
        }

        public static void Beep() => SystemSounds.Beep.Play();

        public static void EditFile(string file) => Process.Start("Notepad.exe", file);

        public static string AssemblyFolder() => Path.GetDirectoryName(ExecutingAssembly());

        public static string ExecutingAssembly() => Assembly.GetExecutingAssembly().GetName().CodeBase.Substring(8).Replace('/', '\\');

        public static void ExportHtmlToClipboard(string markdown, IMarkdownConverter converter)
        {
            var text = RemoveYamlFrontMatter(markdown);
            var html = converter.ConvertToHtml(text, false);
            Clipboard.SetText(html);
        }

        public static void ExportHtmlTemplateToClipboard(string markdown, IMarkdownConverter converter)
        {
            var text = RemoveYamlFrontMatter(markdown);
            var html = converter.ConvertToHtml(text, false);
            html = UserTemplate.InsertContent(html);
            Clipboard.SetText(html);
        }

        public static string RemoveYamlFrontMatter(string markdown)
        {
            if (App.UserSettings.IgnoreYaml == false) return markdown;
            var tuple = SeperateFrontMatter(markdown);
            return tuple.Item2;
        }

        public static Tuple<string, string> SeperateFrontMatter(string text)
        {
            if (Regex.IsMatch(text, @"^---\s*$", RegexOptions.Multiline))
            {
                var matches = Regex.Matches(text, @"^(?:---)|(?:\.\.\.)\s*$", RegexOptions.Multiline);
                if (matches.Count < 2) return Tuple.Create(string.Empty, text);
                var match = matches[1];
                var index = match.Index + match.Groups[0].Value.Length + 1;
                while (index < text.Length && char.IsWhiteSpace(text[index])) index += 1;
                return Tuple.Create(text.Substring(0, index), text.Substring(index));
            }
            return Tuple.Create(string.Empty, text);
        }

        public static T GetDescendantByType<T>(this Visual element) where T : class
        {
            if (element == null) return default(T);
            if (element.GetType() == typeof(T)) return element as T;
            T foundElement = null;
            (element as FrameworkElement)?.ApplyTemplate();
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = visual.GetDescendantByType<T>();
                if (foundElement != null)
                {
                    break;
                }
            }
            return foundElement;
        }

        public static void ShowParseError(Exception ex, string file)
        {
            MessageBox.Show(
                Application.Current.MainWindow,
                $"{ex.Message} in {file}",
                App.Title,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        public static void RequireNotNull<T>(this T arg, [CallerMemberName] string name = null)
        {
            if (arg == null) throw new ArgumentNullException(name);
        }
    }
}