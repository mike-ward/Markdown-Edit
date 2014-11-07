using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using CommonMark;
using MarkdownEdit.Properties;
using mshtml;

namespace MarkdownEdit
{
    public partial class Preview
    {
        public readonly Action<string> UpdatePreview;
        private readonly Func<string, string> _uriResolver = Utility.Memoize<string, string>(UriResolver);
        private readonly FileSystemWatcher _templateWatcher;

        public Preview()
        {
            InitializeComponent();
            Browser.NavigateToString(UserTemplate.Load().Template);
            UpdatePreview = Utility.Debounce<string>(s => Dispatcher.Invoke(() => Update(s)));
            Browser.Navigating += BrowserOnNavigating;
            Browser.PreviewKeyDown += BrowserPreviewKeyDown;
            _templateWatcher = new FileSystemWatcher
            {
                Path = UserSettings.SettingsFolder,
                Filter = Path.GetFileName(UserTemplate.TemplateFile),
                NotifyFilter = NotifyFilters.LastWrite
            };
            _templateWatcher.Changed += (sender, args) => Dispatcher.Invoke(UpdateTemplate);
            _templateWatcher.EnableRaisingEvents = true;
            Unloaded += (sender, args) => _templateWatcher.Dispose();
        }

        private void Update(string markdown)
        {
            if (markdown == null) return;
            string html;
            markdown = RemoveYamlFrontMatter(markdown);
            try
            {
                html = CommonMarkConverter.Convert(markdown, new CommonMarkSettings {UriResolver = _uriResolver});
            }
            catch (CommonMarkException e)
            {
                MessageBox.Show(e.ToString());
                return;
            }
            GetContentsDiv().InnerHtml = html;
        }

        private dynamic GetContentsDiv()
        {
            dynamic document = Browser.Document;
            if (document != null)
            {
                var element = document.GetElementById("content");
                if (element != null) return element;
            }
            return null;
        }

        private void UpdateTemplate()
        {
            var contents = GetContentsDiv().InnerHtml;
            Browser.NavigateToString(UserTemplate.Load().Template);
            GetContentsDiv().InnerHtml = contents;
        }

        private static string UriResolver(string text)
        {
            if (Regex.IsMatch(text, @"^\w+://")) return text;
            var lastOpen = Settings.Default.LastOpenFile;
            if (string.IsNullOrEmpty(lastOpen)) return text;
            var path = Path.GetDirectoryName(lastOpen);
            if (string.IsNullOrEmpty(path)) return text;
            var file = text.TrimStart('/');
            return FindAsset(path, file) ?? text;
        }

        private static string FindAsset(string path, string file)
        {
            try
            {
                var asset = Path.Combine(path, file);
                for (var i = 0; i < 4; ++i)
                {
                    if (File.Exists(asset)) return "file://" + asset.Replace('\\', '/');
                    var parent = Directory.GetParent(path);
                    if (parent == null) break;
                    path = parent.FullName;
                    asset = Path.Combine(path, file);
                }
            }
            catch (ArgumentException)
            {
            }
            return null;
        }

        private static void BrowserOnNavigating(object sender, NavigatingCancelEventArgs ea)
        {
            ea.Cancel = true;
            var url = ea.Uri.ToString();
            if (url.StartsWith("about:", StringComparison.OrdinalIgnoreCase) == false) Process.Start(url);
        }

        private static string RemoveYamlFrontMatter(string markdown)
        {
            if (App.UserSettings.IgnoreYAML == false) return markdown;
            if (markdown.StartsWith("---\n", StringComparison.Ordinal) ||
                markdown.StartsWith("---\r\n", StringComparison.Ordinal))
            {
                var index = Regex.Match(markdown.Substring(3), @"^(---)|(\.\.\.)", RegexOptions.Multiline).Index;
                if (index > 0) return markdown.Substring(index + 6);
            }
            return markdown;
        }

        public void SetScrollOffset(ScrollChangedEventArgs ea)
        {
            if (App.UserSettings.SynchronizeScrollPositions == false) return;
            var document2 = (IHTMLDocument2)Browser.Document;
            var document3 = (IHTMLDocument3)Browser.Document;
            if (document3 != null)
            {
                var percentToScroll = PercentScroll(ea);
                var body = document3.getElementsByTagName("body").item(0);
                var scrollHeight = ((IHTMLElement2)body).scrollHeight - document3.documentElement.offsetHeight;
                document2.parentWindow.scrollTo(0, (int)Math.Round(percentToScroll * scrollHeight));
            }
        }

        private static double PercentScroll(ScrollChangedEventArgs e)
        {
            var y = e.ExtentHeight - e.ViewportHeight;
            return e.VerticalOffset / ((Math.Abs(y) < .000001) ? 1 : y);
        }

        private void BrowserPreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.O:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        ApplicationCommands.Open.Execute(this, Application.Current.MainWindow);
                        e.Handled = true;
                    }
                    break;

                case Key.N:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        ApplicationCommands.New.Execute(this, Application.Current.MainWindow);
                        e.Handled = true;
                    }
                    break;

                case Key.F1:
                    ApplicationCommands.Help.Execute(this, Application.Current.MainWindow);
                    e.Handled = true;
                    break;

                case Key.F5:
                    e.Handled = true;
                    break;
            }
        }
    }
}