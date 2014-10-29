using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
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
                MessageBox.Show(e.Message);
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

        private static string UriResolver(string s)
        {
            if (Regex.IsMatch(s, @"^\w+://")) return s;
            var lastOpen = Settings.Default.LastOpenFile;
            if (string.IsNullOrEmpty(lastOpen)) return s;
            var path = Path.GetDirectoryName(lastOpen);
            if (string.IsNullOrEmpty(path)) return s;
            var file = s.TrimStart('/');
            return FindAsset(path, file) ?? s;
        }

        private static string FindAsset(string path, string file)
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
            return null;
        }

        private static void BrowserOnNavigating(object sender, NavigatingCancelEventArgs ea)
        {
            ea.Cancel = true;
            var url = ea.Uri.ToString();
            if (url.StartsWith("about:", StringComparison.OrdinalIgnoreCase) == false) Process.Start(url);
        }

        public string RemoveYamlFrontMatter(string markdown)
        {
            const string yaml = "---\n";
            const string yaml2 = "---\r\n";
            const string yamlEnd = "\n---";
            if (!markdown.StartsWith(yaml) && !markdown.StartsWith(yaml2)) return markdown;
            var index = markdown.IndexOf(yamlEnd, yaml.Length, StringComparison.Ordinal);
            return (index == -1) ? markdown : markdown.Substring(Math.Min(index + yaml2.Length, markdown.Length));
        }

        public void SetScrollOffset(ScrollChangedEventArgs ea)
        {
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

        //private void BrowserPreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //    switch (e.KeyCode)
        //    {
        //        case Key.O:
        //            if (e.Control == false) break;
        //            ApplicationCommands.Open.Execute(this, Application.Current.MainWindow);
        //            e.IsInputKey = true;
        //            break;

        //        case Key.N:
        //            if (e.Control == false) break;
        //            ApplicationCommands.New.Execute(this, Application.Current.MainWindow);
        //            e.IsInputKey = true;
        //            break;

        //        case Key.F1:
        //            ApplicationCommands.Help.Execute(this, Application.Current.MainWindow);
        //            e.IsInputKey = true;
        //            break;

        //        case Key.F5:
        //            e.IsInputKey = true;
        //            break;
        //    }
        //}
    }
}