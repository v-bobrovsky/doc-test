using Documents.Data;
using Documents.Models;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Documents.Core
{
    /// <summary>
    /// Unity filter provider
    /// </summary>
    public class UnityFilterProvider : IFilterProvider
    {
        private IUnityContainer _container;
        private readonly ActionDescriptorFilterProvider _defaultProvider = new ActionDescriptorFilterProvider();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="container"></param>
        public UnityFilterProvider(IUnityContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// Get filters
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="actionDescriptor"></param>
        /// <returns></returns>
        public IEnumerable<FilterInfo> GetFilters(HttpConfiguration configuration, HttpActionDescriptor actionDescriptor)
        {
            var attributes = _defaultProvider.GetFilters(configuration, actionDescriptor);

            foreach (var attr in attributes)
            {
                _container.BuildUp(attr.Instance.GetType(), attr.Instance);
            }
            return attributes;
        }
    }
}