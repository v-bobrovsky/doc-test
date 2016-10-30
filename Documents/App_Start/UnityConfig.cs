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

        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public static void RegisterComponents(HttpConfiguration config)
        {
			var container = new UnityContainer();

            container.RegisterType<ILogger, SimpleLogger>(new HierarchicalLifetimeManager());
            container.RegisterType<IUserContext, UserContext>(new HierarchicalLifetimeManager());

            IUserContext userCtx = _container.Resolve<IUserContext>();

            container.RegisterType<IDocumentService, DocumentService>(new HierarchicalLifetimeManager(), 
                new InjectionConstructor(userCtx));
            container.RegisterType<ICommentService, CommentService>(new HierarchicalLifetimeManager(), 
                new InjectionConstructor(userCtx));
            container.RegisterType<IUserService, UserService>(new HierarchicalLifetimeManager(), 
                new InjectionConstructor(userCtx));

            config.DependencyResolver = new UnityResolver(container);

            //Register the filter injector
            var providers = config.Services.GetFilterProviders().ToList();
            var defaultprovider = providers.Single(i => i is ActionDescriptorFilterProvider);

            config.Services.Remove(typeof(IFilterProvider), defaultprovider);
            config.Services.Add(typeof(IFilterProvider), new UnityFilterProvider(container));
        }
    }
}