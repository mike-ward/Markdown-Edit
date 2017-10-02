using Prism.Modularity;
using System;
using Microsoft.Practices.Unity;

namespace UserModule
{
    public class UserModule : IModule
    {
        private readonly IUnityContainer _container;

        public UserModule(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
        }
    }
}