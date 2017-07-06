using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Infrastructure;
using mshtml;
using PreviewModule.ViewModels;

namespace PreviewModule.Views
{
    public partial class PreviewControl
    {
        public PreviewControl(ITemplateLoader templateLoader)
        {
            InitializeComponent();
            _browser.Navigate(templateLoader.DefaultTemplate());
            ViewModel.UpdateBrowserDelegate = UpdateBrowser;
            Loaded += KillPopups;
        }

        private PreviewControlViewModel ViewModel => (PreviewControlViewModel) DataContext;

        private void KillPopups(object sender, RoutedEventArgs routedEventArgs)
        {
            Task.Factory.StartNew(() =>
            {
                dynamic activeX = _browser.GetType().InvokeMember("ActiveXInstance",
                    BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                    null, _browser, new object[] { });
                activeX.Silent = true;
            });
        }

        public void UpdateBrowser(string html)
        {
            var div = (_browser.Document as IHTMLDocument3)?.getElementById("content");
            if (div == null)
            {
                _browser.NavigateToString(
                    "<h1 style=\"margin: 40% 25%; text-align: center\">" +
                    "Content element not found<br/>" +
                    "Preview unavailable" +
                    "</h1>");
                return;
            }
            div.innerHTML = html;
        }
    }
}