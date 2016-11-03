using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Documents.DataAccess;
using Documents.Utils;

namespace Documents.Integration.Tests.Core
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
        }
    }
}