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
        public static void RegisterComponents(HttpConfiguration config)
        {
			var container = new UnityContainer();

            container.RegisterType<ILogger, SimpleLogger>(new HierarchicalLifetimeManager());
            container.RegisterType<IDocumentService, DocumentService>(new HierarchicalLifetimeManager());
            container.RegisterType<ICommentService, CommentService>(new HierarchicalLifetimeManager());

            container.RegisterType<IUserService, UserService>();

            config.DependencyResolver = new UnityResolver(container);

            //Register the filter injector
            var providers = config.Services.GetFilterProviders().ToList();
            var defaultprovider = providers.Single(i => i is ActionDescriptorFilterProvider);

            config.Services.Remove(typeof(IFilterProvider), defaultprovider);
            config.Services.Add(typeof(IFilterProvider), new UnityFilterProvider(container));
        }
    }
}