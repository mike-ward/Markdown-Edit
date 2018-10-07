using System;
using System.Windows;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;

namespace MarkdownEdit
{
    public partial class App : PrismApplication
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<Shell>();
        }

        protected override Window CreateShell()
        {
            var shell = Container.Resolve<Shell>();
            return shell;
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            AddModule(moduleCatalog, typeof(ServicesModule.ServicesModule));
            AddModule(moduleCatalog, typeof(UserModule.UserModule));
            AddModule(moduleCatalog, typeof(EditModule.EditModule));
            AddModule(moduleCatalog, typeof(PreviewModule.PreviewModule));
        }

        private void AddModule(IModuleCatalog moduleCatalog, Type moduleType)
        {
            moduleCatalog.AddModule(new ModuleInfo
            {
                ModuleName = moduleType.Name,
                ModuleType = moduleType.AssemblyQualifiedName,
                InitializationMode = InitializationMode.WhenAvailable
            });
        }
    }
}
