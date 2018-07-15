using System.Collections.Generic;
using System.Windows;
using Infrastructure;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Unity;
using UserModule.Commands;
using UserModule.Views;

namespace UserModule
{
    public class UserModule : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public UserModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        private void RegisterTypes()
        {
            _container.RegisterType<IUserCommandHandler, DisplaySettingsCommandHandler>(nameof(DisplaySettingsCommandHandler));
            _container.RegisterType<IUserCommandHandler, HelpCommandHandler>(nameof(HelpCommandHandler));

            _container.RegisterType<IEnumerable<IUserCommandHandler>, IUserCommandHandler[]>();
        }

        private void AddCommandHandlers()
        {
            var shell = (Window)_regionManager.Regions[Constants.EditRegion].Context;
            var commandHandlers = _container.Resolve<IUserCommandHandler[]>();

            foreach (var commandHandler in commandHandlers)
            {
                commandHandler.Initialize(shell);
            }
        }

        private void AddViews()
        {
            _regionManager.RegisterViewWithRegion(Constants.WindowCommandsRegion, typeof(CommandPanel));
            _regionManager.RegisterViewWithRegion(Constants.FlyoutControlsRegion, typeof(SettingsFlyout));
            _regionManager.RegisterViewWithRegion(Constants.FlyoutControlsRegion, typeof(DocumentStructureFlyout));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            RegisterTypes();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            AddCommandHandlers();
            AddViews();
        }
    }
}