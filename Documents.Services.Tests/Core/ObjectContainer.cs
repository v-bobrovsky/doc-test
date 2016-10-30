using Documents.DataAccess;
using Documents.Services.Impl;
using Documents.Utils;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documents.Services.Tests
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

        /// <summary>
        /// Constructor
        /// </summary>
        static ObjectContainer()
        {
            _container.RegisterType<ILogger, SimpleLogger>();
            _container.RegisterType<IUnitOfWork, UnitOfWork>();
            _container.RegisterType<IUserContext, TestUserContext>();

            IUserContext userCtx = _container.Resolve<IUserContext>();

            _container.RegisterType<IDocumentService, DocumentService>(new HierarchicalLifetimeManager(),
                new InjectionConstructor(userCtx));
            _container.RegisterType<ICommentService, CommentService>(new HierarchicalLifetimeManager(),
                new InjectionConstructor(userCtx));
            _container.RegisterType<IUserService, UserService>(new HierarchicalLifetimeManager(),
                new InjectionConstructor(userCtx));
        }
    }
}