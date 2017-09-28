using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using Infrastructure;

namespace EditModule.Views
{
    public partial class HelpDialog
    {
        private readonly IMarkdownEngine _markdownEngine;

        public HelpDialog(IMarkdownEngine markdownEngine)
        {
            _markdownEngine = markdownEngine;
            InitializeComponent();
            Loaded += KillPopups;
            Loaded += OnLoaded;
            SourceInitialized += (sd, ea) =>  Globals.Tracker.Configure(this).Apply();
            Closed += (sd, ea) => Globals.Tracker.Configure(this).Persist();
            KeyDown += (sd, ea) => { if (ea.Key == Key.F1) Close(); };
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var helpText = Properties.Resources.Help;
            var html = _markdownEngine.ToHtml(helpText);
            Browser.NavigateToString(html);
            Dispatcher.InvokeAsync(() => Browser.Navigating += BrowserOnNavigating);
        }

        private void KillPopups(object sender, RoutedEventArgs routedEventArgs)
        {
            Dispatcher.InvokeAsync(() =>
            {
                dynamic activeX = Browser.GetType().InvokeMember("ActiveXInstance",
                    BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                    null, Browser, new object[] { });
                activeX.Silent = true;
            });
        }

        private static void BrowserOnNavigating(object sender, NavigatingCancelEventArgs ea)
        {
            ea.Cancel = true;
            var url = ea.Uri?.ToString();
            if (!string.IsNullOrWhiteSpace(url)) Process.Start(url);
        }
    }
}
