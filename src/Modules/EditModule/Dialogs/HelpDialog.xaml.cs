using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Infrastructure;
using ServicesModule;

namespace EditModule.Dialogs
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
            Closed += (sd, ea) => Settings.Tracker.Configure(this).Persist();
            KeyDown += (sd, ea) => { if (ea.Key == Key.F1) Close(); };
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var helpText = Properties.Resources.Help;
            var html = _markdownEngine.ToHtml(helpText);
            Browser.NavigateToString(html);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            Settings.Tracker.Configure(this).Apply();
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
    }
}
