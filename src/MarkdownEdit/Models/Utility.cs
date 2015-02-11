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

        public static Action<T> Debounce<T>(this Action<T> func, int milliseconds = 300)
        {
            var last = 0;
            return arg =>
            {
                var current = Interlocked.Increment(ref last);
                Task.Delay(milliseconds).ContinueWith(task =>
                {
                    if (current == last) func(arg);
                    task.Dispose();
                });
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
            if (markdown.StartsWith("---\n", StringComparison.Ordinal) ||
                markdown.StartsWith("---\r\n", StringComparison.Ordinal))
            {
                var index = Regex.Match(markdown.Substring(3), @"^(---)|(\.\.\.)", RegexOptions.Multiline).Index;
                if (index > 0) return markdown.Substring(index + 6);
            }
            return markdown;
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

        public static FileSystemWatcher WatchFile(string file, Action onChange)
        {
            var fileWatcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(file),
                Filter = Path.GetFileName(file),
                NotifyFilter = NotifyFilters.LastWrite,
            };
            fileWatcher.Changed += (sender, args) =>
            {
                // Suggested method to "debounce" multiple notifications
                try
                {
                    fileWatcher.EnableRaisingEvents = false;
                    onChange();
                }
                finally
                {
                    fileWatcher.EnableRaisingEvents = true;
                }
            };
            fileWatcher.EnableRaisingEvents = true;
            return fileWatcher;
        }

        public static void ShowParseError(Exception ex, string file)
        {
            MessageBox.Show(
                Application.Current.MainWindow,
                string.Format("{0} in {1}", ex.Message, file),
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