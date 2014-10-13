using System;
using System.Windows;
using MarkdownDeep;

namespace MarkdownEdit
{
    public partial class Preview
    {
        public Preview()
        {
            InitializeComponent();
            SizeChanged += OnSizeChanged;
            Browser.DocumentText = Properties.Resources.GithubTemplateHtml;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            BrowserHost.Width = ActualWidth;
            BrowserHost.Height = ActualHeight;
        }

        public void UpdatePreview(string markdown)
        {
            if (markdown == null) return;
            markdown = RemoveYamlFrontMatter(markdown);
            var md = new Markdown();
            var html = md.Transform(markdown);
            var document = Browser.Document;
            if (document != null)
            {
                var element = document.GetElementById("content");
                if (element != null) element.InnerHtml = html;
            }
        }

        public string RemoveYamlFrontMatter(string markdown)
        {
            const string yaml = "---\n";
            const string yaml2 = "---\r\n";
            const string yamlEnd = "\n---";
            if (!markdown.StartsWith(yaml) && !markdown.StartsWith(yaml2)) return markdown;
            var index = markdown.IndexOf(yamlEnd, yaml.Length, StringComparison.Ordinal);
            return (index == -1) ? markdown : markdown.Substring(index + yaml2.Length);
        }

        public void SetScrollOffset(int offset)
        {
            var document = Browser.Document;
            if (document != null)
            {
                var body = document.Body;
                if (body != null) body.ScrollTop = offset;
            }
        }
    }
}