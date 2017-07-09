using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Infrastructure;
using mshtml;
using PreviewModule.ViewModels;
using Prism.Events;

namespace PreviewModule.Views
{
    public partial class PreviewControl
    {
        public ITemplateLoader TemplateLoader { get; }
        public IEventAggregator EventAggregator { get; }
        private PreviewControlViewModel ViewModel => (PreviewControlViewModel)DataContext;

        public PreviewControl(ITemplateLoader templateLoader, IEventAggregator eventAggregator)
        {
            TemplateLoader = templateLoader;
            EventAggregator = eventAggregator;
            InitializeComponent();
            ViewModel.UpdateBrowserDelegate = UpdateBrowser;
            _browser.PreviewKeyDown += BrowserPreviewKeyDown;
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

        private void BrowserPreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.O:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        EventAggregator.GetEvent<OpenCommandEvent>().Publish();
                        e.Handled = true;
                    }
                    break;

                case Key.N:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        EventAggregator.GetEvent<NewCommandEvent>().Publish();
                        e.Handled = true;
                    }
                    break;

                case Key.F1:
                    EventAggregator.GetEvent<HelpCommandEvent>().Publish();
                    e.Handled = true;
                    break;
            }
        }
    }
}