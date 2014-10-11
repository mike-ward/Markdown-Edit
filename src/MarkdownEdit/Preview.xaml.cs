using System;
using System.Windows;

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
    }
}