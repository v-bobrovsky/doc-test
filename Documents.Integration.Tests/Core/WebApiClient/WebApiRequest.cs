using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Documents.Integration.Tests.Core
{
    /// <summary>
    /// Web api request
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WebApiRequest<T>
        where T : class
    {
        #region Members

        public string Controller { get; private set; }
        public HttpMethod HttpMethod { get; private set; }
        public T Value { get; set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="httpMethod"></param>
        /// <param name="value"></param>
        public WebApiRequest(string controller, HttpMethod httpMethod, T value = null)
        {
            this.Controller = controller;
            this.HttpMethod = httpMethod;
            this.Value = value;
        }
    }
}
