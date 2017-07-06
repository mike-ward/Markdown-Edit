using Infrastructure;
using Microsoft.Practices.Unity;
using Prism.Modularity;

namespace ServicesModule
{
    public class ServicesModule : IModule
    {
        public IUnityContainer Container { get; }

        public ServicesModule(IUnityContainer container)
        {
            Container = container;
        }

        public void Initialize()
        {
            Container.RegisterType<IMarkdownEngine, CommonMarkEngine>();
            Container.RegisterType<INotify, Notify>();
            Container.RegisterType<IOpenSaveActions, OpenSaveActions>();
            Container.RegisterType<ISettings, Settings>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ITemplateLoader, TemplateLoader>();
        }
    }
}