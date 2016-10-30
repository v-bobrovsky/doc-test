using System;
using System.Web.Http.Controllers;

using Documents.Core;
using Documents.Services;
using Microsoft.Practices.Unity;

namespace Documents.Filters
{
    public class DocumentPermissions : BaseContentPermissions
    {
        [Dependency]
        public IDocumentService DocumentService { get; set; }

        protected override bool CheckIsOwnContent(HttpActionContext actionContext)
        {
            var documentId = (Guid)actionContext.ActionArguments["id"];
            return DocumentService.CheckIsDocumentOwner(documentId);
        }
    }
}