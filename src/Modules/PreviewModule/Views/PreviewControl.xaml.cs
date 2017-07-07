using System.Reflection;
using System.Windows;
using Infrastructure;
using mshtml;
using PreviewModule.ViewModels;

namespace PreviewModule.Views
{
    public partial class PreviewControl
    {
        public ITemplateLoader TemplateLoader { get; }
        private PreviewControlViewModel ViewModel => (PreviewControlViewModel)DataContext;

        public PreviewControl(ITemplateLoader templateLoader)
        {
            TemplateLoader = templateLoader;
            InitializeComponent();
            ViewModel.UpdateBrowserDelegate = UpdateBrowser;
            Loaded += KillPopups;
            Loaded += LoadTemplate;
        }

        private void KillPopups(object sender, RoutedEventArgs routedEventArgs)
        {
            Dispatcher.InvokeAsync(() =>
            {
                dynamic activeX = _browser.GetType().InvokeMember("ActiveXInstance",
                    BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                    null, _browser, new object[] { });
                activeX.Silent = true;
            });
        }

        private void LoadTemplate(object sender, RoutedEventArgs routedEventArgs)
        {
            Dispatcher.InvokeAsync(() => _browser.Navigate(TemplateLoader.DefaultTemplate()));
        }

        public void UpdateBrowser(string html)
        {
            var div = (_browser.Document as IHTMLDocument3)?.getElementById("content");
            if (div == null)
            {
                ShowTemplateError();
            }
            else
            {
                div.innerHTML = html;
            }
        }

        private void ShowTemplateError()
        {
            _browser.NavigateToString(
                "<h1 style=\"margin: 40% 25%; text-align: center\">" +
                "Content element not found<br/>" +
                "Preview unavailable" +
                "</h1>");
        }
    }
}