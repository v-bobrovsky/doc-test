using System;
using System.Web.Http.Controllers;

using Documents.Core;

namespace Documents.Filters
{
    public class UserPermissions: BaseContentPermissions
    {
        protected override bool CheckIsOwnContent(HttpActionContext actionContext)
        {
            var userId = 0;
            return ((int)actionContext.ActionArguments["id"] == userId);
        }
    }
}