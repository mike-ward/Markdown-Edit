using Prism.Regions;

namespace MarkdownEdit.Views
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
