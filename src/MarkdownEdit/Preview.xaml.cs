using System;
using System.Windows;
using CommonMark;
using Point = System.Drawing.Point;

namespace MarkdownEdit
{
    public partial class Preview
    {
        public Preview()
        {
            InitializeComponent();
            Browser.DocumentText = Properties.Resources.GithubTemplateHtml;
        }

        public void UpdatePreview(string markdown)
        {
            if (markdown == null) return;
            string html;
            markdown = RemoveYamlFrontMatter(markdown);
            try
            {
                html = CommonMarkConverter.Convert(markdown);
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