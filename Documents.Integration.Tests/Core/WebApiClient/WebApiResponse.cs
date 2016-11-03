using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Documents.Integration.Tests.Core
{
    /// <summary>
    /// Web api response
    /// </summary>
    public class WebApiResponse
    {
        #region Members

        public HttpStatusCode Code
        {
            set;
            get;
        }

        public string Response
        {
            set;
            get;
        }

        public bool Result
        {
            set;
            get;
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public WebApiResponse()
            : this(HttpStatusCode.NoContent, string.Empty, false)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="code"></param>
        /// <param name="response"></param>
        /// <param name="result"></param>
        public WebApiResponse(HttpStatusCode code, string response, bool result)
        {
            Code = code;
            Response = response;
            Result = result;
        }

        public T To<T>()
        {
            var result = default(T);

            if (!String.IsNullOrEmpty(Response))
            {
                try
                {
                    result = JsonConvert
                        .DeserializeObject<T>(Response);
                }
                catch
                {
                    result = default(T);
                }
            }

            return result;
        }
    }
}