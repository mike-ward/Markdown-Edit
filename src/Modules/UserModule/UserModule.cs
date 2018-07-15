using System.Collections.Generic;
using System.Windows;
using Infrastructure;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using UserModule.Commands;
using UserModule.Views;

namespace UserModule
{
    public class UserModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public UserModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IUserCommandHandler, DisplaySettingsCommandHandler>(nameof(DisplaySettingsCommandHandler));
            containerRegistry.Register<IUserCommandHandler, HelpCommandHandler>(nameof(HelpCommandHandler));
            containerRegistry.Register<IEnumerable<IUserCommandHandler>, IUserCommandHandler[]>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            AddCommandHandlers(containerProvider);
            AddViews();
        }

        private void AddCommandHandlers(IContainerProvider containerProvider)
        {
            var shell = (Window)_regionManager.Regions[Constants.EditRegion].Context;
            var commandHandlers = containerProvider.Resolve<IUserCommandHandler[]>();

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
    }
}