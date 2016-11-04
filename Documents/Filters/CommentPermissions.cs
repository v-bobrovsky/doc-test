using System;
using System.Web.Http.Controllers;

using Documents.Core;
using Documents.Services;
using Microsoft.Practices.Unity;

namespace Documents.Filters
{
    /// <summary>
    /// Provides main functionality to implement security permissions for comments
    /// </summary>
    public class CommentPermissions : BaseContentPermissions
    {
        [Dependency]
        public ICommentService CommentService { get; set; }

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
                var id = actionContext
                    .RequestContext
                    .RouteData
                    .Values["id"] as string;

                var commentId = 0;

                if (!int.TryParse(id, out commentId))
                    commentId = 0;

                result = CommentService
                    .CheckIsCommentOwner(commentId);
            }

            return result;
        }
    }
}