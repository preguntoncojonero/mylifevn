using System;
using Microsoft.Practices.Unity;

namespace MyLife.Web
{
    public static class ServiceManager
    {
        private static readonly IUnityContainer servicesProviderContainer = new UnityContainer();

        public static void RegisterService<S>(Type serviceType)
        {
            var instance = Activator.CreateInstance(serviceType);
            servicesProviderContainer.RegisterInstance(typeof(S), instance);
        }

        public static void RegisterService<S>(object instance)
        {
            servicesProviderContainer.RegisterInstance(typeof(S), instance);
        }

        public static S GetService<S>() where S : class
        {
            return servicesProviderContainer.Resolve(typeof (S)) as S;
        }
    }
}