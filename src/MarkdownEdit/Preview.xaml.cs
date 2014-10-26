using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using CommonMark;
using MarkdownEdit.Properties;
using mshtml;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

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
            Browser.DocumentText = UserTemplate.Load().Template;
            UpdatePreview = Utility.Debounce<string>(s => Dispatcher.Invoke(() => Update(s)));
            Browser.Navigating += BrowserOnNavigating;
            Browser.GetType().InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, Browser, new object[] {true});

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
            UpdateContentsDiv(html);
        }

        private void UpdateContentsDiv(string html)
        {
            var document = Browser.Document;
            if (document != null)
            {
                var element = document.GetElementById("content");
                if (element != null) element.InnerHtml = html;
            }
        }

        private void UpdateTemplate()
        {
            var content = Browser.Document.GetElementById("content").InnerHtml;
            Browser.Document.Write(UserTemplate.Load().Template);
            Browser.Document.GetElementById("content").InnerHtml = content;
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

        private void BrowserOnNavigating(object sender, WebBrowserNavigatingEventArgs ea)
        {
            ea.Cancel = true;
            var url = ea.Url.ToString();
            if (url.StartsWith("about:", StringComparison.OrdinalIgnoreCase) == false) Process.Start(ea.Url.ToString());
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
            var document = Browser.Document;
            if (document != null && document.Window != null && document.Body != null)
            {
                var percentToScroll = PercentScroll(ea);
                var documentElement = ((IHTMLDocument3)document.DomDocument).documentElement;
                var scrollHeight = document.Body.OffsetRectangle.Height - documentElement.offsetHeight;
                document.Window.ScrollTo(0, (int)Math.Round(percentToScroll * scrollHeight));
            }
        }

        private static double PercentScroll(ScrollChangedEventArgs e)
        {
            var y = e.ExtentHeight - e.ViewportHeight;
            return e.VerticalOffset / ((Math.Abs(y) < .000001) ? 1 : y);
        }

        private void BrowserPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.O:
                    if (e.Control == false) break;
                    ApplicationCommands.Open.Execute(this, Application.Current.MainWindow);
                    e.IsInputKey = true;
                    break;

                case Keys.N:
                    if (e.Control == false) break;
                    ApplicationCommands.New.Execute(this, Application.Current.MainWindow);
                    e.IsInputKey = true;
                    break;

                case Keys.F1:
                    ApplicationCommands.Help.Execute(this, Application.Current.MainWindow);
                    e.IsInputKey = true;
                    break;

                case Keys.F5:
                    e.IsInputKey = true;
                    break;
            }
        }
    }
}