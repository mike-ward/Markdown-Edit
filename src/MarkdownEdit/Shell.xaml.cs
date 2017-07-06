using System;
using Infrastructure;
using Prism.Regions;

namespace MarkdownEdit
{
    public partial class Shell
    {
        public IRegionManager RegionManager { get; }

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
        }
    }
}
