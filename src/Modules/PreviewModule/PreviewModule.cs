using Infrastructure;
using PreviewModule.Views;
using Prism.Modularity;
using Prism.Regions;

namespace PreviewModule
{
    public class PreviewModule : IModule
    {
        public IRegionManager RegionManager { get; }

        public PreviewModule(IRegionManager regionManager)
        {
            RegionManager = regionManager;
        }

        public void Initialize()
        {
            RegionManager.RegisterViewWithRegion(Constants.PreviewRegion, typeof(PreviewControl));
        }
    }
}