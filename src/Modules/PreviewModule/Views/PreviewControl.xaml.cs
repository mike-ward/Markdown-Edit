using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using mshtml;
using PreviewModule.ViewModels;

namespace PreviewModule.Views
{
    public partial class PreviewControl
    {
        public PreviewControl()
        {
            InitializeComponent();
            ViewModel.UpdateBrowserDelegate = UpdateBrowser;
            Loaded += OnLoaded;
            //_browser.Navigating += BrowserOnNavigating;
        }

        private PreviewControlViewModel ViewModel => (PreviewControlViewModel)DataContext;

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Task.Factory.StartNew(() =>
            {
                // kill popups
                dynamic activeX = _browser.GetType().InvokeMember("ActiveXInstance",
                    BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                    null, _browser, new object[] { });
                activeX.Silent = true;
            });
        }

        public void UpdateBrowser(string html)
        {
            _browser.NavigateToString(string.IsNullOrEmpty(html) ? "&nbsp" : html);
        }

        private void BrowserOnNavigating(object sender, NavigatingCancelEventArgs ea)
        {
            ea.Cancel = true;
            var url = ea.Uri?.ToString();
            if (url?.StartsWith("file://", StringComparison.OrdinalIgnoreCase) == true) NavigateToElement(ea.Uri.OriginalString);
            if (url?.StartsWith("about:", StringComparison.OrdinalIgnoreCase) == false) Process.Start(url);
        }

        private void NavigateToElement(string url)
        {
            var lastIndex = url.LastIndexOf("#", StringComparison.Ordinal);
            if (lastIndex == -1) return;
            var id = url.Substring(lastIndex + 1);
            var document3 = _browser.Document as IHTMLDocument3;
            if (document3 == null) return;
            var element =
                document3.getElementById(id) ??
                document3.getElementsByName("a").Cast<IHTMLElement>().FirstOrDefault(e => e.getAttribute("name") == id);
            if (element == null) return;
            var document2 = _browser.Document as IHTMLDocument2;
            document2?.parentWindow.scroll(0, element.offsetTop);
        }
    }
}
