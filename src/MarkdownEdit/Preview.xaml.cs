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
            var html = md.Transform(markdown);
            Browser.NavigateToString(html);
        }
    }
}