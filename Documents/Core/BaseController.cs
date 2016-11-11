using System;
using System.Web;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using System.Security.Claims;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using System.Web.Http.Description;
using Microsoft.Owin.Security.OAuth;
using System.Security.Authentication;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Practices.Unity;

using Documents.Utils;
using Documents.Data;

namespace Documents.Core
{
    /// <summary>
    /// Base controller to be used by all controllers
    /// </summary>
    public class BaseController : ApiController
    {
        private ServiceUserManager _serviceUserManager;

        [ApiExplorerSettings(IgnoreApi = true)]
        public ServiceUserManager ServiceUserManager
        {
            get
            {
                return _serviceUserManager ??
                       (_serviceUserManager = Request
                       .GetOwinContext()
                       .GetUserManager<ServiceUserManager>());
            }
        }

        protected IAuthenticationManager Authentication
        {
            get
            {
                return Request
                    .GetOwinContext()
                    .Authentication;
            }
        }

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

        /// <summary>
        /// Get logged current user unique identity
        /// </summary>
        /// <returns></returns>
        protected int GetCurrentUserId()
        {
            var result = 0;

            if (User != null &&
                User.Identity != null)
            {
                var userId = User
                    .Identity
                    .GetUserId();

                if (!Int32.TryParse(userId, out result))
                    result = 0;
            }

            return result;
        }

        /// <summary>
        /// Get permissions context for logged current user
        /// </summary>
        /// <returns></returns>
        protected PermissionsContext GetPermissionsContext()
        {
            var userId = GetCurrentUserId();
            return new PermissionsContext(userId);
        }
    }
}