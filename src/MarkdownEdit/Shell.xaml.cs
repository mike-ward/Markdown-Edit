using System;
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
            RegionManager.Regions["EditRegion"].Context = this;
            RegionManager.Regions["PreviewRegion"].Context = this;
        }
    }
}
