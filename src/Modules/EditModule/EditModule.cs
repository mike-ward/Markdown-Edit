using EditModule.Views;
using ICSharpCode.AvalonEdit;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace EditModule
{
    public class EditModule : IModule
    {
        public IUnityContainer Container { get; }
        public IRegionManager RegionManager { get; }

        public EditModule(IUnityContainer container, IRegionManager regionManager)
        {
            Container = container;
            RegionManager = regionManager;
        }

        public void Initialize()
        {
            Container.RegisterType<ITextEditorComponent, TextEditor>();
            RegionManager.RegisterViewWithRegion("EditRegion", typeof(EditControl));
        }
    }
}
