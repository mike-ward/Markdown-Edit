using Prism.Regions;

namespace MarkdownEdit.Views
{
    public partial class MainWindow 
    {
        public MainWindow(IRegionManager regionManager)
        {
            InitializeComponent();
            regionManager.RegisterViewWithRegion("ContentRegion", typeof(MainView));
        }
    }
}
