using System.Collections.Generic;
using System.Windows;
using Infrastructure;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using UserModule.Commands;

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

        public void Initialize()
        {
            RegisterTypes();
            AddCommandHandlers();
        }

        private void RegisterTypes()
        {
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
    }
}