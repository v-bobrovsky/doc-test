using System;
using Microsoft.Practices.Unity;
using System.Web.Http.Controllers;

using Documents.Core;
using Documents.Services;
using System.Web;

namespace Documents.Filters
{
    public class UserPermissions : BaseContentPermissions
    {
        /// <summary>
        /// Get current user Id
        /// </summary>
        protected int GetCurrentUserId()
        {
            var result = 0;

            if (HttpContext.Current != null &&
                HttpContext.Current.User != null &&
                HttpContext.Current.User.Identity != null)
            {
                var userId = HttpContext
                    .Current
                    .User
                    .Identity
                    .GetUserId<string>();

                if (!Int32.TryParse(userId, out result))
                    result = 0;
            }

            return result;
        }

        protected override bool CheckIsOwnContent(HttpActionContext actionContext)
        {
            var userId = GetCurrentUserId();
            return ((int)actionContext.ActionArguments["id"] == userId);
        }
    }
}