using System;
using System.IO;
using System.Runtime;
using System.Windows;
using Infrastructure;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;

namespace MarkdownEdit
{
    public partial class App : PrismApplication
    {
        public App()
        {
            // Enable Multi-JIT startup
            var profileRoot = Globals.UserSettingsFolder;
            Directory.CreateDirectory(profileRoot);
            ProfileOptimization.SetProfileRoot(profileRoot);
            ProfileOptimization.StartProfile("Startup.profile");
        }

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
