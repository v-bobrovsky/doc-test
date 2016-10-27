using System;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Documents.Filters
{
    public abstract class BaseContentPermissions: AuthorizeAttribute
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
        /// <param name="userId"></param>
        /// <returns></returns>
        protected abstract bool CheckIsOwnContent(HttpActionContext actionContext, int userId);

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var result = base.IsAuthorized(actionContext);

            if (result)
            {
                if (!String.IsNullOrEmpty(Roles))
                    allowedRoles = Roles.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (allowedRoles == null)
                    allowedRoles = new string[0];

                result = //actionContext.Request.IsAuthenticated &&
                     IsInRole(actionContext) && IsContentAccess(actionContext);

            }

            return result;
        }

        /// <summary>
        /// Check 
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        private bool IsContentAccess(HttpActionContext actionContext)
        {
            var result = true;

            if (!String.IsNullOrEmpty(ContentOwner) && ContentOwner.ToLower().Trim() == "own")
                result = CheckIsOwnContent(actionContext, 0);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        private bool IsInRole(HttpActionContext actionContext)
        {
            var result = true;

            if (allowedRoles.Length > 0)
            {
                //result = false;

                //for (int i = 0; i < allowedRoles.Length; i++)
                //{
                //    if (actionContext.User.IsInRole(allowedRoles[i]))
                //    {
                //        result = true;
                //        break;
                //    }
                //}
            }

            return result;
        }
    }
}