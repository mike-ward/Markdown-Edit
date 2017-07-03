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
            Container.RegisterType<IFileActions, FileActions>();
            Container.RegisterType<IMarkdownEngine, CommonMarkEngine>();
            Container.RegisterType<IMessageBox, MessageBox>();
        }
    }
}