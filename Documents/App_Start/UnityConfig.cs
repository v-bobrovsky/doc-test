using Documents.Core;
using Documents.Services;
using Documents.Services.Impl;
using Documents.Utils;
using Microsoft.Practices.Unity;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Filters;
using Unity.WebApi;

namespace Documents
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        private static readonly IUnityContainer _container = new UnityContainer();

        /// <summary>
        /// Resolve instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        static UnityConfig()
        {
            _container.RegisterType<ILogger, SimpleLogger>(new HierarchicalLifetimeManager());
            _container.RegisterType<IUserContext, UserContext>();

            IUserContext userCtx = _container.Resolve<IUserContext>();

            _container.RegisterType<IDocumentService, DocumentService>(new HierarchicalLifetimeManager(),
                new InjectionConstructor(userCtx));
            _container.RegisterType<ICommentService, CommentService>(new HierarchicalLifetimeManager(),
                new InjectionConstructor(userCtx));
            _container.RegisterType<IUserService, UserService>(new HierarchicalLifetimeManager(),
                new InjectionConstructor(userCtx));
        }

        /// <summary>
        /// Register components
        /// </summary>
        /// <param name="config"></param>
        public static void RegisterComponents(HttpConfiguration config)
        {
            config.DependencyResolver = new UnityResolver(_container);

            //Register the filter injector
            var providers = config.Services.GetFilterProviders().ToList();
            var defaultprovider = providers.Single(i => i is ActionDescriptorFilterProvider);

            config.Services.Remove(typeof(IFilterProvider), defaultprovider);
            config.Services.Add(typeof(IFilterProvider), new UnityFilterProvider(_container));
        }
    }
}