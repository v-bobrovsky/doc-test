using System;
using System.Web.Http.Controllers;

namespace Documents.Filters
{
    public class UserPermissions: BaseContentPermissions
    {
        protected override bool CheckIsOwnContent(HttpActionContext actionContext, int userId)
        {
            return ((int)actionContext.ActionArguments["id"] == userId);
        }
    }
}