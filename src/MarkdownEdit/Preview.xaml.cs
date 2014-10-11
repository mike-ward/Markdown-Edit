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
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            Browser.Width = ActualWidth - 2;
            Browser.Height = ActualHeight - 2;
        }

        public void UpdatePreview(string markdown)
        {
            if (markdown == null) Browser.NavigateToString(string.Empty);
            var md = new Markdown();
            markdown = RemoveYamlFrontMatter(markdown);
            var html = md.Transform(markdown);
            var doc = Properties.Resources.GithubTemplateHtml.Replace("**content**", html);
            Browser.NavigateToString(doc);
        }

        public string RemoveYamlFrontMatter(string markdown)
        {
            const string yaml = "---\n";
            const string yaml2 = "---\r\n";
            const string yamlEnd = "\n---";
            if (!markdown.StartsWith(yaml) && !markdown.StartsWith(yaml2)) return markdown;
            var index = markdown.IndexOf(yamlEnd, yaml.Length, System.StringComparison.Ordinal);
            return (index == -1) ? markdown : markdown.Substring(index + yaml2.Length);
        }
    }
}