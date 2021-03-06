using Microsoft.Practices.Unity;
using System.Web.Http;

using Unity.WebApi;
using Documents.DataAccess;
using Documents.Utils;

namespace Documents.Services.Impl
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
            _container.RegisterType<ILogger, SimpleLogger>();
            _container.RegisterType<IUnitOfWork, UnitOfWork>();
        }
    }
}