using System.Linq;
using System.Web.Http;
using System.Web.Http.Filters;

using Unity.WebApi;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

using Documents.Core;
using Documents.Utils;
using Documents.Common;
using Documents.Services;
using Documents.Services.Impl;

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
            _container.RegisterType<ILogger, SimpleLogger>();

            _container.AddNewExtension<Interception>();

            _container.RegisterType<IDocumentService, DocumentService>(new Interceptor<VirtualMethodInterceptor>(),
                                        new InterceptionBehavior<LoggerInterceptionBehavior>());

            _container.RegisterType<ICommentService, CommentService>(new Interceptor<VirtualMethodInterceptor>(),
                                        new InterceptionBehavior<LoggerInterceptionBehavior>());

            _container.RegisterType<IUserService, UserService>(new Interceptor<VirtualMethodInterceptor>(),
                                        new InterceptionBehavior<LoggerInterceptionBehavior>());
        }

        /// <summary>
        /// Register components
        /// </summary>
        /// <param name="config"></param>
        public static void RegisterComponents(HttpConfiguration config)
        {
            config.DependencyResolver = new UnityResolver(_container);
        }
    }
}