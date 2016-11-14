using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

using Documents.Utils;
using Documents.Common;
using Documents.DataAccess;
using Documents.Services.Impl;

namespace Documents.Services.Tests.Core
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

            _container.AddNewExtension<Interception>();

            _container.RegisterType<IDocumentService, DocumentService>(new Interceptor<VirtualMethodInterceptor>(),
                                        new InterceptionBehavior<LoggerInterceptionBehavior>());

            _container.RegisterType<ICommentService, CommentService>(new Interceptor<VirtualMethodInterceptor>(),
                                        new InterceptionBehavior<LoggerInterceptionBehavior>());

            _container.RegisterType<IUserService, UserService>(new Interceptor<VirtualMethodInterceptor>(),
                                        new InterceptionBehavior<LoggerInterceptionBehavior>());
        }
    }
}