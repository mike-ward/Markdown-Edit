using PreviewModule.ViewModels;

namespace PreviewModule.Views
{
    public partial class PreviewControl
    {
        public PreviewControl()
        {
            InitializeComponent();
            ((PreviewControlViewModel)DataContext).UpdateBrowserDelegate = UpdateBrowser;
        }

        public void UpdateBrowser(string html)
        {
            _browser.NavigateToString(html);
        }
    }
}
