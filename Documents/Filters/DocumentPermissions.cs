using System;
using System.Web.Http.Controllers;

using Documents.Core;
using Documents.Services;
using Microsoft.Practices.Unity;
using Documents.Data;

namespace Documents.Filters
{
    /// <summary>
    /// Provides main functionality to implement security permissions for documents
    /// </summary>
    public class DocumentPermissions : BaseContentPermissions
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

                var documentId = Guid
                    .Empty;

                if (!Guid.TryParse(id, out documentId))
                    documentId = Guid
                    .Empty;

                if (!documentId.Equals(Guid.Empty))
                {
                    var service = UnityConfig.Resolve<IDocumentService>();

                    result =
                        service
                        .CheckIsDocumentOwner(
                            new PermissionsContext(userId),
                            documentId);
                }
                else
                {
                    result = true;
                }
            }

            return result;
        }
    }
}