using System;
using System.Web.Http;

using Documents.Utils;
using Microsoft.Practices.Unity;
using System.Text;

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
                Logger.LogInfo(String.Format("Performing controller - {0} for action - {1}",
                    this.GetType().Name, action.Method.Name));

                if (ModelState.IsValid)
                {
                    var retAction = action();

                    if (retAction != null)
                    {
                        result = Ok<T>(retAction);
                    }                       
                    else
                    {
                        result = NotFound();
                        Logger.LogError("Data not found!");
                    }
                        
                }
                else
                {
                    var sbValidationErrors = new StringBuilder();

                    sbValidationErrors.AppendLine("Validation data error(s):");

                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            sbValidationErrors.AppendLine(error.ErrorMessage);
                        }
                    }

                    Logger.LogError(sbValidationErrors
                        .ToString());

                    result = BadRequest(ModelState);
                }
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