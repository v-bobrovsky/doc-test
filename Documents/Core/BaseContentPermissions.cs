using System;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity.Owin;

namespace Documents.Core
{
    /// <summary>
    /// Provides base functionality to implement security permissions for specific content
    /// </summary>
    public abstract class BaseContentPermissions : AuthorizeAttribute
    {
        #region Members

        /// <summary>
        /// Define content owner: "Any" or "Own".
        /// "Any" or empty uses by default
        /// </summary>
        public string ContentOwner { get; set; }

        /// <summary>
        /// Allowed user roles
        /// </summary>
        protected string[] allowedRoles;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public BaseContentPermissions()
        {
        }

        /// <summary>
        /// Check is own content
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected abstract bool CheckIsOwnContent(HttpActionContext actionContext);

        /// <summary>
        /// Indicates whether the specified control is authorized.
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var result = base
                .IsAuthorized(actionContext);

            if (result)
            {
                if (!String.IsNullOrEmpty(Roles))
                    allowedRoles = Roles
                        .Split(new char[1] { ',' }, 
                                StringSplitOptions.RemoveEmptyEntries);

                if (allowedRoles == null)
                    allowedRoles = new string[0];


                result = IsCurrentUserAuthenticated() && 
                    IsInRole(actionContext) && 
                    IsContentAccess(actionContext);
            }

            return result;
        }

        /// <summary>
        /// Check have user permissions to access for content
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns>
        /// True if successfullly otherwise False
        /// </returns>
        private bool IsContentAccess(HttpActionContext actionContext)
        {
            var result = true;

            if (!String.IsNullOrEmpty(ContentOwner) && ContentOwner.ToLower().Trim() == "own")
                result = CheckIsOwnContent(actionContext);

            return result;
        }

        /// <summary>
        /// Check is authenticated current user
        /// </summary>
        /// <returns>
        /// True if authenticated otherwise False
        /// </returns>
        private bool IsCurrentUserAuthenticated()
        {
            var result = false;

            if (HttpContext.Current != null &&
                HttpContext.Current.User != null &&
                HttpContext.Current.User.Identity != null)
            {
                result = HttpContext
                    .Current
                    .User
                    .Identity
                    .IsAuthenticated;
            }

            return result;
        }

        /// <summary>
        /// Get current user role
        /// </summary>
        /// <returns></returns>
        private string GetCurrentUserRole()
        {
            var result = string.Empty;

            if (HttpContext.Current != null &&
                HttpContext.Current.User != null &&
                HttpContext.Current.User.Identity != null)
            {
                result = HttpContext
                    .Current
                    .User
                    .Identity
                    .GetUserRole();
            }

            return result;
        }

        /// <summary>
        /// Check is user have a specific role
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns>
        /// True if user have a specific role
        /// </returns>
        private bool IsInRole(HttpActionContext actionContext)
        {
            var result = true;

            if (allowedRoles.Length > 0)
            {
                result = false;

                var currentUserRole = GetCurrentUserRole();

                if (!String.IsNullOrEmpty(currentUserRole))
                {
                    for (int i = 0; i < allowedRoles.Length; i++)
                    {
                        if (allowedRoles[i].ToLower().Trim() == currentUserRole.ToLower().Trim())
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }

            return result;
        }
    }
}