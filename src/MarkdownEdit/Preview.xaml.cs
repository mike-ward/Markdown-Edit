using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using CommonMark;
using MarkdownEdit.Properties;
using Point = System.Drawing.Point;

namespace MarkdownEdit
{
    public partial class Preview
    {
        public readonly Action<string> UpdatePreview;
        private readonly Func<string, string> _uriResolver = Utility.Memoize<string, string>(UriResolver);

        public Preview()
        {
            InitializeComponent();
            Browser.DocumentText = Properties.Resources.GithubTemplateHtml;
            UpdatePreview = Utility.Debounce<string>(s => Dispatcher.Invoke(() => Update(s)));
        }

        public void Update(string markdown)
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
            var document = Browser.Document;
            if (document != null)
            {
                var element = document.GetElementById("content");
                if (element != null) element.InnerHtml = html;
            }
        }

        private static string UriResolver(string s)
        {
            if (Regex.IsMatch(s, @"^\w+://")) return s;
            var lastOpen = Settings.Default.LastOpenFile;
            if (string.IsNullOrEmpty(lastOpen)) return s;
            var path = Path.GetDirectoryName(lastOpen);
            if (string.IsNullOrEmpty(path)) return s;
            var file = s.TrimStart('/');
            var asset = Path.Combine(path, file);

            for (var i = 0; i < 4; ++i)
            {
                if (File.Exists(asset)) return "file://" + asset.Replace('\\', '/');
                var parent = Directory.GetParent(path);
                if (parent == null) break;
                path = parent.FullName;
                asset = Path.Combine(path, file);
            }
            return s;
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

        public void SetScrollOffset(int offset)
        {
            var document = Browser.Document;
            if (document != null)
            {
                var window = document.Window;
                if (window != null) window.ScrollTo(new Point(0, offset));
            }
        }
    }
}