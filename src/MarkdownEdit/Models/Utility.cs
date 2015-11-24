using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using MarkdownEdit.Controls;
using MarkdownEdit.i18n;
using Microsoft.Win32;

namespace MarkdownEdit.Models
{
    public static class Utility
    {
        public const string Version = "1.16.2";

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

        public static Process EditFile(string file) => App.UserSettings.UseDefaultEditor
            ? Process.Start(file)
            : Process.Start("Notepad.exe", file);

        public static string AssemblyFolder() => Path.GetDirectoryName(ExecutingAssembly());

        public static string ExecutingAssembly() => Assembly.GetExecutingAssembly().GetName().CodeBase.Substring(8).Replace('/', '\\');

        public static void ExportHtmlToClipboard(string markdown, bool includeTemplate = false)
        {
            var text = RemoveYamlFrontMatter(markdown);
            var html = Markdown.ToHtml(text);
            if (includeTemplate) html = UserTemplate.InsertContent(html);
            CopyHtmlToClipboard(html);
        }

        private static void CopyHtmlToClipboard(string html)
        {
            Clipboard.SetText(html);
            var popup = new FadingPopupControl();
            var message = TranslationProvider.Translate("message-html-clipboard") as string;
            popup.ShowDialogBox(Application.Current.MainWindow, message);
        }

        public static string RemoveYamlFrontMatter(string markdown)
        {
            if (App.UserSettings.IgnoreYaml == false) return markdown;
            var tuple = Markdown.SeperateFrontMatter(markdown);
            return tuple.Item2;
        }

        public static T GetDescendantByType<T>(this Visual element) where T : class
        {
            if (element == null) return default(T);
            if (element.GetType() == typeof (T)) return element as T;
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
            Application.Current.Dispatcher.Invoke(() =>
                Application.Current.MainWindow != null
                    ? MessageBox.Show(
                        Application.Current.MainWindow,
                        $"{ex.Message} in {file}",
                        App.Title,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error)
                    : MessageBox.Show(
                        $"{ex.Message} in {file}",
                        App.Title,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error)
                );
        }

        public static async Task<bool> IsCurrentVersion()
        {
            try
            {
                using (var http = new HttpClient())
                {
                    var version = await http.GetStringAsync("http://markdownedit.com/version.txt");
                    return string.IsNullOrWhiteSpace(version) || version == Version;
                }
            }
            catch (Exception)
            {
                return true;
            }
        }
    }
}