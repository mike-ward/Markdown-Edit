using Prism.Regions;

namespace MainModule.Views
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
