using System;
using System.ComponentModel;
using Infrastructure;
using Prism.Regions;
using ServicesModule;

namespace MarkdownEdit
{
    public partial class Shell
    {
        public IRegionManager RegionManager { get; }
        private ShellViewModel ViewModel => (ShellViewModel)DataContext;

        public Shell(IRegionManager regionManager)
        {
            RegionManager = regionManager;
            InitializeComponent();
            Activated += OnActivated;
        }

        private void OnActivated(object sender, EventArgs eventArgs)
        {
            Activated -= OnActivated;
            RegionManager.Regions[Constants.EditRegion].Context = this;
            RegionManager.Regions[Constants.PreviewRegion].Context = this;
            Dispatcher.InvokeAsync(() => ViewModel.UpdateAppTitle());
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            Settings.Tracker.Configure(this).Apply();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = !ViewModel.AskToSaveIfModified();
        }
    }
}
