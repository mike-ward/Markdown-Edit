using System;

namespace PreviewModule.Views
{
    public partial class PreviewControl
    {
        public PreviewControl()
        {
            InitializeComponent();
            _browser.Navigate(new Uri("http://example.com"));
        }
    }
}
