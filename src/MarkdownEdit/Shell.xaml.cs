using System;
using System.ComponentModel;
using System.Windows;
using Infrastructure;
using Prism.Regions;

namespace MarkdownEdit
{
    public partial class Shell
    {
        public IRegionManager RegionManager { get; }
        private ShellViewModel ViewModel => (ShellViewModel)DataContext;
        private bool _shutdown;

        public Shell(IRegionManager regionManager)
        {
            RegionManager = regionManager;
            InitializeComponent();
            Activated += OnActivated;
            SourceInitialized += (sd, ea) => Globals.Tracker.Track(this);

            Globals.Tracker.Configure<Shell>()
                .Properties(w => new { w.Width, w.Height, w.WindowState });
        }

        private void OnActivated(object sender, EventArgs eventArgs)
        {
            Activated -= OnActivated;
            RegionManager.Regions[Constants.EditRegion].Context = this;
            RegionManager.Regions[Constants.PreviewRegion].Context = this;
            Dispatcher.InvokeAsync(() =>
            {
                ViewModel.UpdateAppTitle();
            });
        }

        protected override async void OnClosing(CancelEventArgs e)
        {
            if (_shutdown) return;

            e.Cancel = true;
            _shutdown = await ViewModel.AskToSaveIfModified();

            if (_shutdown)
            {
                Globals.Tracker.PersistAll();
                Application.Current.Shutdown();
            }
        }
    }
}