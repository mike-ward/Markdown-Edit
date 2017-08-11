using System.Collections.Generic;
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
            Container.RegisterType<IAbstractSyntaxTree, AbstractSyntaxTree>();
            Container.RegisterType<IColorService, ColorService>();
            Container.RegisterType<INotify, Notify>();
            Container.RegisterType<IOpenSaveActions, OpenSaveActions>();
            Container.RegisterType<ISettings, Settings>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IStrings, Strings>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ITemplateLoader, TemplateLoader>();

            Container.RegisterType<IMarkdownEngine, CommonMarkEngine>("CommonMark");
            Container.RegisterType<IEnumerable<IMarkdownEngine>, IMarkdownEngine[]>();
        }
    }
}