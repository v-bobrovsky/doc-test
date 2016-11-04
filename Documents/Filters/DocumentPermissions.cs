using System;
using System.Web.Http.Controllers;

using Documents.Core;
using Documents.Services;
using Microsoft.Practices.Unity;

namespace Documents.Filters
{
    /// <summary>
    /// Provides main functionality to implement security permissions for documents
    /// </summary>
    public class DocumentPermissions : BaseContentPermissions
    {
        [Dependency]
        public IDocumentService DocumentService { get; set; }

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

                var documentId = Guid
                    .Empty;

                if (!Guid.TryParse(id, out documentId))
                    documentId = Guid
                    .Empty;

                if (!documentId.Equals(Guid.Empty))
                    result =
                        DocumentService
                        .CheckIsDocumentOwner(documentId);
                else
                    result = true;
            }

            return result;
        }
    }
}