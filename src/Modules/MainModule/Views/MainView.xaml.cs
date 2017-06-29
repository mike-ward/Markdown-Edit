using EditModule.Views;
using PreviewModule.Views;
using Prism.Regions;

namespace MainModule.Views
{
    public partial class MainView
    {
        public MainView(IRegionManager regionManager)
        {
            InitializeComponent();
            regionManager.RegisterViewWithRegion("EditorRegion", typeof(EditControl));
            regionManager.RegisterViewWithRegion("PreviewRegion", typeof(PreviewControl));
        }
    }
}
