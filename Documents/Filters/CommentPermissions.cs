using System;
using System.Web.Http.Controllers;

using Documents.Services;
using Microsoft.Practices.Unity;

namespace Documents.Filters
{
    public class CommentPermissions : BaseContentPermissions
    {
        [Dependency]
        public ICommentService CommentService { get; set; }

        protected override bool CheckIsOwnContent(HttpActionContext actionContext, int userId)
        {
            var commentId = (int)actionContext.ActionArguments["id"];
            return CommentService.CheckIsCommentOwner(commentId);
        }
    }
}