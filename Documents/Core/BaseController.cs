using System;
using System.Web.Http;

using Documents.Utils;
using Microsoft.Practices.Unity;

namespace Documents.Core
{
    /// <summary>
    /// Base controller to be used by all controllers
    /// </summary>
    public class BaseController : ApiController
    {
        [Dependency]
        public ILogger Logger { get; set; }

        /// <summary>
        /// Performs the specified action and returns IHttpActionResult
        /// </summary>
        /// <param name="action">action to perform</param>
        /// <returns>Action result</returns>
        protected IHttpActionResult PerformAction<T>(Func<T> action)
        {
            IHttpActionResult result;

            try
            {
                Logger.LogInfo(String.Format("Performing controller action: {0}", action.Method.Name));

                var retAction = action();

                if (retAction != null)
                    result = Ok<T>(retAction);
                else
                    result = NotFound();
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                result = InternalServerError(e);
            }

            return result;
        }
    }
}