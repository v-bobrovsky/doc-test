using Microsoft.Practices.Unity;
using System.Web.Http;

using Unity.WebApi;
using Documents.DataAccess;
using Documents.Utils;
using Documents.Services;
using Documents.Services.Impl;

namespace Documents.Tests.Core
{
    /// <summary>
    /// Helper class for unity DI
    /// </summary>
    public static class ObjectContainer
    {
        private static readonly IUnityContainer _container = new UnityContainer();

        /// <summary>
        /// Resolve instance
        /// </summary>
        /// <typeparam name="T">Instance type</typeparam>
        /// <returns></returns>
        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        static ObjectContainer()
        {
            _container.RegisterType<ILogger, SimpleLogger>(new HierarchicalLifetimeManager());
            _container.RegisterType<IUnitOfWork, UnitOfWork>(new HierarchicalLifetimeManager());
            _container.RegisterType<IUserContext, UserContext>(new HierarchicalLifetimeManager());

            _container.RegisterType<IDocumentService, DocumentService>(new HierarchicalLifetimeManager());
            _container.RegisterType<ICommentService, CommentService>(new HierarchicalLifetimeManager());
            _container.RegisterType<IUserService, UserService>(new HierarchicalLifetimeManager());

        }
    }
}