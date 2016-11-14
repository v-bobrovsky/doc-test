using System;
using System.Web.Http.Controllers;

using Documents.Core;
using Documents.Services;
using Microsoft.Practices.Unity;
using Documents.Data;

namespace Documents.Filters
{
    /// <summary>
    /// Provides main functionality to implement security permissions for comments
    /// </summary>
    public class CommentPermissions : BaseContentPermissions
    {
        /// <summary>
        /// Check is own content
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected override bool CheckIsOwnContent(HttpActionContext actionContext)
        {
            var result = false;

            if (actionContext != null &&
                actionContext.RequestContext != null &&
                actionContext.RequestContext.RouteData != null &&
                actionContext.RequestContext.RouteData.Values != null)
            {
                var userId = GetUserId(actionContext);

                var id = actionContext
                    .RequestContext
                    .RouteData
                    .Values["id"] as string;

                var commentId = 0;

                if (!int.TryParse(id, out commentId))
                    commentId = 0;

                if (commentId > 0)
                {
                    var service = UnityConfig.Resolve<ICommentService>();

                    result = service
                        .CheckIsCommentOwner(
                            new PermissionsContext(userId),
                            commentId);
                }
            }

            return result;
        }
    }
}