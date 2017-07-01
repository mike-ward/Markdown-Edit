using PreviewModule.ViewModels;

namespace PreviewModule.Views
{
    public partial class PreviewControl
    {
        public PreviewControl()
        {
            InitializeComponent();
            ViewModel.UpdateBrowserDelegate = UpdateBrowser;
        }

        private PreviewControlViewModel ViewModel => (PreviewControlViewModel)DataContext;

        public void UpdateBrowser(string html)
        {
            _browser.NavigateToString(html);
        }
    }
}
