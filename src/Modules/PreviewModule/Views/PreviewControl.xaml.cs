using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Infrastructure;
using mshtml;
using PreviewModule.ViewModels;

namespace PreviewModule.Views
{
    public partial class PreviewControl
    {
        private readonly ISettings _settings;
        public ITemplateLoader TemplateLoader { get; }
        private PreviewControlViewModel ViewModel => (PreviewControlViewModel)DataContext;

        public PreviewControl(ITemplateLoader templateLoader, ISettings settings)
        {
            _settings = settings;
            TemplateLoader = templateLoader;
            InitializeComponent();
            ViewModel.UpdateBrowserDelegate = UpdateBrowser;
            ViewModel.ScrollToOffsetDelegate = ScrollToAnchor;
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
            Dispatcher.Invoke(() => _browser.Navigate(TemplateLoader.DefaultTemplate()));
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
                UpdateBaseTag();
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

        private void UpdateBaseTag()
        {
            const string basetTagId = "base-tag-id";
            var lastOpen = _settings.CurrentFileName.StripOffsetFromFileName();
            if (string.IsNullOrWhiteSpace(lastOpen)) return;
            var folder = Path.GetDirectoryName(lastOpen);
            if (string.IsNullOrWhiteSpace(folder)) return;
            var document = _browser.Document as IHTMLDocument3;
            var baseElement = document?.getElementById(basetTagId);
            if (baseElement == null)
            {
                var doc2 = _browser.Document as IHTMLDocument2;
                baseElement = doc2?.createElement("base");
                if (baseElement == null) return;
                baseElement.id = basetTagId;
                var head = document?.getElementsByTagName("head").item(0);
                head?.appendChild(baseElement);
            }
            baseElement.setAttribute("href", "file:///" + folder.Replace('\\', '/') + "/");
        }

        private static void BrowserPreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.O:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        ApplicationCommands.Open.Execute(null, Application.Current.MainWindow);
                        e.Handled = true;
                    }
                    break;

                case Key.N:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        ApplicationCommands.New.Execute(null, Application.Current.MainWindow);
                        e.Handled = true;
                    }
                    break;

                case Key.F1:
                    ApplicationCommands.Help.Execute(null, Application.Current.MainWindow);
                    e.Handled = true;
                    break;
            }
        }

        private static string GetIdName(int number) => $"mde-{number}";

        private void ScrollToAnchor(int number, int extra)  
        {
            var document3 = _browser.Document as IHTMLDocument3;
            if (document3?.documentElement != null)
            {
                var offsetTop = 0;
                var document2 = _browser.Document as IHTMLDocument2;
                if (number == int.MaxValue)
                {
                    if (document2?.body is IHTMLElement2 body) offsetTop = body.scrollHeight;
                }
                else if (number > 1)
                {
                    var element = document3.getElementById(GetIdName(number));
                    if (element == null) return;
                    offsetTop = element.offsetTop + extra * 20;
                }
                document2?.parentWindow.scroll(0, offsetTop);
            }
        }
    }
}