using System;
using MainModule.Views;
using Microsoft.Practices.Unity;
using Prism.Modularity;

namespace MainModule
{
    public class MainModule : IModule
    {
        private readonly IModuleCatalog _moduleCatalog;

        public MainModule(IModuleCatalog moduleCatalog)
        {
            _moduleCatalog = moduleCatalog;
        }

        public void Initialize()
        {
            AddModule(typeof(EditModule.EditModule));
            AddModule(typeof(PreviewModule.PreviewModule));
        }

        private void AddModule(Type moduleType)
        {
            _moduleCatalog.AddModule(new ModuleInfo
            {
                ModuleName = moduleType.Name,
                ModuleType = moduleType.AssemblyQualifiedName,
                InitializationMode = InitializationMode.WhenAvailable
            });
        }
    }
}
