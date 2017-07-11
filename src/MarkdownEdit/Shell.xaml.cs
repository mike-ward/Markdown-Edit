using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Infrastructure;
using MahApps.Metro.Controls;
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
            AddRightWindowCommands();
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

        private void AddRightWindowCommands()
        {
            var newButton = new Button()
            {
                FontFamily = new FontFamily(Constants.SymbolFont),
                Content = char.ConvertFromUtf32(0xe160),
                Command = ApplicationCommands.New
            };

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(20, 0, 20, 0)
            };

            stackPanel.Children.Add(newButton);

            RightWindowCommands = new WindowCommands {ShowLastSeparator = false};
            RightWindowCommands.Items.Add(stackPanel);
        }
    }
}
