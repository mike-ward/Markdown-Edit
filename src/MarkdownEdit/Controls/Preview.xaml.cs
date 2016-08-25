using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using HtmlAgilityPack;
using mshtml;
using MarkdownEdit.Commands;
using MarkdownEdit.Converters;
using MarkdownEdit.Models;
using MarkdownEdit.Properties;
using System.Linq;

// ReSharper disable RedundantCaseLabel

namespace MarkdownEdit.Controls
{
    public partial class Preview : INotifyPropertyChanged
    {
        public enum StatisticMode
        {
            Character,
            Word,
            Page
        }

        public readonly Action<Editor> UpdatePreview;
        private int _characterCount;
        private string _documentStatistics;
        private string _documentStatisticsToolTip;
        private int _pageCount;
        private FileSystemWatcher _templateWatcher;
        private int _wordCount;

        public Preview()
        {
            InitializeComponent();
            Browser.Navigate(new Uri(UserTemplate.Load()));
            Loaded += OnLoaded;
            Unloaded += (sender, args) => _templateWatcher?.Dispose();
            Browser.Navigating += BrowserOnNavigating;
            Browser.PreviewKeyDown += BrowserPreviewKeyDown;
            Browser.MessageHook += BrowserOnMessageHook;
            Browser.Unloaded += (s, e) => ApplicationCommands.Close.Execute(null, Application.Current.MainWindow);
            UpdatePreview = Utility.Debounce<Editor>(editor => Dispatcher.InvokeAsync(() => Update(editor.Text)));
            DocumentStatisticMode = StatisticMode.Word;
        }

        public StatisticMode DocumentStatisticMode { get; set; }

        public int WordCount
        {
            get { return _wordCount; }
            set { Set(ref _wordCount, value); }
        }

        public int CharacterCount
        {
            get { return _characterCount; }
            set { Set(ref _characterCount, value); }
        }

        public string DocumentStatisticDisplayText
        {
            get { return _documentStatistics; }
            set { Set(ref _documentStatistics, value); }
        }

        public string DocumentStatisticsToolTip
        {
            get { return _documentStatisticsToolTip; }
            set { Set(ref _documentStatisticsToolTip, value); }
        }

        public int PageCount
        {
            get { return _pageCount; }
            set { Set(ref _pageCount, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Task.Factory.StartNew(() =>
            {
                _templateWatcher = UserTemplate.TemplateFile.WatchFile(() => Dispatcher.Invoke(UpdateTemplate));

                // kill popups
                dynamic activeX = Browser.GetType().InvokeMember("ActiveXInstance",
                    BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                    null, Browser, new object[] { });

                activeX.Silent = true;
            });
        }

        private async void Update(string markdown)
        {
            if (markdown == null) return;
            try
            {
                UpdateBaseTag();
                var div = GetContentsDiv();
                if (div == null) return;

                await Task.Run(() =>
                {
                    var html = Markdown.ToHtml(markdown);
                    div.innerHTML = ScrubHtml(html);
                });

                UpdateDocumentStatistics(div.innerText ?? string.Empty);
                EmitFirePreviewUpdatedEvent();
            }
            catch (Exception ex)
            {
                Notify.Alert(ex.ToString());
            }
        }

        public void UpdateDocumentStatistics(string innerText)
        {
            CharacterCount = innerText.Length;
            WordCount = innerText.WordCount();
            PageCount = (int)Math.Ceiling(CharacterCount / 1500m);
            UpdateDocumentStatisticDisplayText();
        }

        public void UpdateDocumentStatisticDisplayText()
        {
            Func<int, string, string> convert = (count, suffix) =>
                new NumberFormatConverter().Convert(count, typeof(string), null,
                    CultureInfo.CurrentCulture) + suffix;

            switch (DocumentStatisticMode)
            {
                case StatisticMode.Character:
                    DocumentStatisticDisplayText = convert(CharacterCount, "c");
                    DocumentStatisticsToolTip = $"{convert(WordCount, "w")}, {convert(PageCount, "p")}";
                    break;
                case StatisticMode.Page:
                    DocumentStatisticDisplayText = convert(PageCount, "p");
                    DocumentStatisticsToolTip = $"{convert(WordCount, "w")}, {convert(CharacterCount, "c")}";
                    break;
                case StatisticMode.Word:
                default:
                    DocumentStatisticDisplayText = convert(WordCount, "w");
                    DocumentStatisticsToolTip = $"{convert(CharacterCount, "c")}, {convert(PageCount, "p")}";
                    break;
            }
        }

        private void EmitFirePreviewUpdatedEvent()
        {
            try
            {
                dynamic doc = Browser.Document;
                if (doc == null) return;
                var ev = doc.createEvent("event");
                if (ev == null) return;
                ev.initEvent("previewUpdated", true, true);
                doc.dispatchEvent(ev);
            }
            catch (COMException)
            {
            }
        }

        private void UpdateBaseTag()
        {
            const string basetTagId = "base-tag-id";
            var lastOpen = Settings.Default.LastOpenFile.StripOffsetFromFileName();
            if (string.IsNullOrWhiteSpace(lastOpen)) return;
            var folder = Path.GetDirectoryName(lastOpen);
            if (string.IsNullOrWhiteSpace(folder)) return;
            var document = Browser.Document as IHTMLDocument3;
            var baseElement = document?.getElementById(basetTagId);
            if (baseElement == null)
            {
                var doc2 = Browser.Document as IHTMLDocument2;
                baseElement = doc2?.createElement("base");
                if (baseElement == null) return;
                baseElement.id = basetTagId;
                var head = document?.getElementsByTagName("head").item(0);
                head?.appendChild(baseElement);
            }
            baseElement.setAttribute("href", "file:///" + folder.Replace('\\', '/') + "/");
        }

        public void Print()
        {
            var document = Browser.Document as IHTMLDocument2;
            document?.execCommand("Print", true);
        }

        private static string GetIdName(int number) => $"mde-{number}";

        private static string ScrubHtml(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            Action<HtmlNodeCollection, Action<HtmlNode>> each = (nodes, action) =>
            {
                if (nodes == null) return;
                foreach (var node in nodes) action.Invoke(node);
            };

            var idx = 1;
            Func<string> getName = () => GetIdName(idx++);

            // Inject anchors at all block level elements for scroll synchronization
            var nc = doc.DocumentNode.SelectNodes("//p|//h1|//h2|//h3|//h4|//h5|//h6|//ul|//ol|//li|//hr|//pre|//blockquote");
            each(nc, node => node.Attributes.Add("id", getName()));

            // Remove potentially harmful elements
            nc = doc.DocumentNode.SelectNodes("//script|//link|//iframe|//frameset|//frame|//applet|//object|//embed");
            each(nc, node => node.ParentNode.RemoveChild(node, false));

            // Remove hrefs to java/j/vbscript URLs
            nc = doc.DocumentNode.SelectNodes("//a[starts-with(translate(@href, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'javascript')]|//a[starts-with(translate(@href, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'jscript')]|//a[starts-with(translate(@href, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'vbscript')]");
            each(nc, node => node.SetAttributeValue("href", "#"));

            // Remove img with refs to java/j/vbscript URLs
            nc = doc.DocumentNode.SelectNodes("//img[starts-with(translate(@src, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'javascript')]|//img[starts-with(translate(@src, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'jscript')]|//img[starts-with(translate(@src, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'vbscript')]");
            each(nc, node => node.SetAttributeValue("src", "#"));

            // Remove on<Event> handlers from all tags
            nc = doc.DocumentNode.SelectNodes("//*[@onclick or @onmouseover or @onfocus or @onblur or @onmouseout or @ondoubleclick or @onload or @onunload]");
            each(nc, node =>
            {
                node.Attributes.Remove("onFocus");
                node.Attributes.Remove("onBlur");
                node.Attributes.Remove("onClick");
                node.Attributes.Remove("onMouseOver");
                node.Attributes.Remove("onMouseOut");
                node.Attributes.Remove("onDoubleClick");
                node.Attributes.Remove("onLoad");
                node.Attributes.Remove("onUnload");
            });

            // remove any style attributes that contain the word expression (IE evaluates this as script)
            nc = doc.DocumentNode.SelectNodes("//*[contains(translate(@style, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'expression')]");
            each(nc, node => node.Attributes.Remove("style"));

            return doc.DocumentNode.WriteTo();
        }

        private IHTMLElement GetContentsDiv()
        {
            var document = Browser.Document as IHTMLDocument3;
            var element = document?.getElementById("content");
            return element;
        }

        private void UpdateTemplate()
        {
            Browser.Refresh();
            UpdatePreviewCommand.Command.Execute(null, this);
        }

        private void BrowserOnNavigating(object sender, NavigatingCancelEventArgs ea)
        {
            ea.Cancel = true;
            var url = ea.Uri?.ToString();
            if (url?.StartsWith("file://", StringComparison.OrdinalIgnoreCase) == true) NavigateToElement(ea.Uri.OriginalString);
            else if (url?.StartsWith("about:", StringComparison.OrdinalIgnoreCase) == false) Process.Start(url);
        }

        private void NavigateToElement(string url)
        {
            var lastIndex = url.LastIndexOf("#", StringComparison.Ordinal);
            if (lastIndex == -1) return;
            var id = url.Substring(lastIndex + 1);
            var document3 = Browser.Document as IHTMLDocument3;
            if (document3 == null) return;
            var element = 
                document3.getElementById(id) ??
                document3.getElementsByName("a").Cast<IHTMLElement>().FirstOrDefault(e => e.getAttribute("name") == id);
            if (element == null) return;
            var document2 = Browser.Document as IHTMLDocument2;
            document2?.parentWindow.scroll(0, element.offsetTop);
        }

        public void SetScrollOffset(object sender, ScrollChangedEventArgs ea)
        {
            if (!IsVisible) return;
            if (App.UserSettings.SynchronizeScrollPositions == false) return;

            var document3 = Browser.Document as IHTMLDocument3;
            if (document3?.documentElement != null)
            {
                var editor = sender as Editor;
                if (editor == null) return;
                var numberAndExtra = editor.VisibleBlockNumber();
                var number = numberAndExtra.Item1;
                var extra = numberAndExtra.Item2;
                var offsetTop = 0;
                var document2 = Browser.Document as IHTMLDocument2;
                if (number == int.MaxValue)
                {
                    var body = document2?.body as IHTMLElement2;
                    if (body != null) offsetTop = body.scrollHeight;
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

        private void BrowserPreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.O:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        ApplicationCommands.Open.Execute(this, Application.Current.MainWindow);
                        e.Handled = true;
                    }
                    break;

                case Key.N:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        ApplicationCommands.New.Execute(this, Application.Current.MainWindow);
                        e.Handled = true;
                    }
                    break;

                case Key.F1:
                    ApplicationCommands.Help.Execute(this, Application.Current.MainWindow);
                    e.Handled = true;
                    break;
            }
        }

        private static IntPtr BrowserOnMessageHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x021:
                    Application.Current.MainWindow.Activate();
                    handled = true;
                    break;
            }

            return hwnd;
        }

        private void Set<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value)) return;
            property = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}