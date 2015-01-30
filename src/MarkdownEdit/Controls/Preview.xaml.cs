using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using CommonMark;
using mshtml;
using MarkdownEdit.MarkdownConverters;
using MarkdownEdit.Models;

namespace MarkdownEdit.Controls
{
    public partial class Preview : INotifyPropertyChanged
    {
        public readonly Action<string> UpdatePreview;
        private FileSystemWatcher _templateWatcher;
        private int _wordCount;

        public Preview()
        {
            InitializeComponent();
            Unloaded += (sender, args) => _templateWatcher?.Dispose();
            Browser.Navigate(UserTemplate.Load());
            UpdatePreview = Utility.Debounce<string>(s => Dispatcher.InvokeAsync(() => Update(s)));
            Browser.Navigating += BrowserOnNavigating;
            Browser.PreviewKeyDown += BrowserPreviewKeyDown;

            Task.Factory.StartNew(() =>
            {
                _templateWatcher = Utility.WatchFile(UserTemplate.TemplateFile, () => Dispatcher.Invoke(UpdateTemplate));

                // kill popups
                dynamic activeX = Browser.GetType().InvokeMember("ActiveXInstance",
                    BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                    null, Browser, new object[] {});

                activeX.Silent = true;
            });
        }

        private void Update(string markdown)
        {
            if (markdown == null) return;
            try
            {
                markdown = Utility.RemoveYamlFrontMatter(markdown);
                var html = MarkdownConverter.ConvertToHtml(markdown);
                var div = GetContentsDiv();
                div.InnerHtml = html;
                WordCount = (div.InnerText as string).WordCount();
            }
            catch (CommonMarkException e)
            {
                MessageBox.Show(e.ToString(), App.Title);
            }
        }

        private dynamic GetContentsDiv()
        {
            dynamic document = Browser.Document;
            if (document != null)
            {
                var element = document.GetElementById("content");
                if (element != null) return element;
            }
            return null;
        }

        private void UpdateTemplate()
        {
            Browser.Refresh();
            MainWindow.UpdatePreviewCommand.Execute(null, this);
        }

        private static void BrowserOnNavigating(object sender, NavigatingCancelEventArgs ea)
        {
            ea.Cancel = true;
            var url = ea.Uri?.ToString();
            if (url?.StartsWith("about:", StringComparison.OrdinalIgnoreCase) == false) Process.Start(url);
        }

        public void SetScrollOffset(ScrollChangedEventArgs ea)
        {
            if (App.UserSettings.SynchronizeScrollPositions == false) return;
            var document2 = (IHTMLDocument2)Browser.Document;
            var document3 = (IHTMLDocument3)Browser.Document;
            if (document3?.documentElement != null)
            {
                var percentToScroll = PercentScroll(ea);
                if (percentToScroll > 0.99) percentToScroll = 1.1; // deal with round off at end of scroll
                var body = document3.getElementsByTagName("body").item(0);
                var scrollHeight = ((IHTMLElement2)body).scrollHeight - document3.documentElement.offsetHeight;
                document2.parentWindow.scrollTo(0, (int)Math.Ceiling(percentToScroll * scrollHeight));
            }
        }

        private static double PercentScroll(ScrollChangedEventArgs e)
        {
            var y = e.ExtentHeight - e.ViewportHeight;
            return e.VerticalOffset / ((Math.Abs(y) < .000001) ? 1 : y);
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

                case Key.F5:
                    e.Handled = true;
                    break;
            }
        }

        // Properties

        public int WordCount
        {
            get { return _wordCount; }
            set { Set(ref _wordCount, value); }
        }

        public static readonly DependencyProperty MarkdownConverterProperty = DependencyProperty.Register(
            "MarkdownConverter", typeof (IMarkdownConverter), typeof (Preview), new PropertyMetadata(default(IMarkdownConverter)));

        public IMarkdownConverter MarkdownConverter
        {
            get { return (IMarkdownConverter)GetValue(MarkdownConverterProperty); }
            set { SetValue(MarkdownConverterProperty, value); }
        }

        public static readonly DependencyProperty HidePreviewProperty = DependencyProperty.Register(
            "HidePreview", typeof (bool), typeof (Preview), new PropertyMetadata(default(bool), HidePreviewPropertyChanged));

        private static void HidePreviewPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var preview = (Preview)dependencyObject;
            if (preview.HidePreview == false)
            {
                Task.Factory.StartNew(() => Task
                    .Delay(100)
                    .ContinueWith(t => preview.Dispatcher.Invoke(() => preview.Browser.Visibility = Visibility.Visible)));
            }
            else
            {
                preview.Browser.Visibility = Visibility.Hidden;
            }
        }

        public bool HidePreview
        {
            get { return (bool)GetValue(HidePreviewProperty); }
            set { SetValue(HidePreviewProperty, value); }
        }

        // INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value) == false)
            {
                property = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}