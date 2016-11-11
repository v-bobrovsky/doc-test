using System;
using System.Collections.Generic;
using System.Web.Http;

using Documents.Core;
using Documents.Data;
using Documents.Models;
using Documents.Services;
using Documents.Filters;

namespace Documents.Controllers
{
    /// <summary>
    /// Provides CRUD functionality for documents.
    /// </summary>
    [Authorize]
    public class DocumentsController : BaseController
    {
        private readonly IDocumentService _documentService;

        public DocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        /// <summary>
        /// Retrieves list of all documents.
        /// GET: api/Documents
        /// </summary>
        /// <returns>
        /// <see cref="IEnumerable&lt;DocumentDto&gt;"/>s object containing the list of documents.
        /// </returns>
        [HttpGet]
        public IHttpActionResult Retrive()
        {
            return PerformAction<IEnumerable<DocumentDto>>(() =>
            {
                var ctx = GetPermissionsContext();

                return _documentService
                    .GetAll(ctx);
            });
        }

        /// <summary>
        /// Retrives a specific document by identity.
        /// GET: api/Documents/543D3EC2-BD1F-4AD1-9DAA-D37BE8375893
        /// </summary>
        /// <param name="id">Document identity</param>
        /// <returns>
        /// <see cref="DocumentDto"/>s object containing the document dto entity.
        /// </returns>
        [HttpGet]
        public IHttpActionResult Retrive(Guid id)
        {
            return PerformAction<DocumentDto>(() =>
            {
                var ctx = GetPermissionsContext();

                return _documentService
                    .Get(ctx, id);
            });
        }

        /// <summary>
        /// Creates a new document.
        /// POST: api/Documents
        /// </summary>
        /// <param name="data">Document data. Name field is mandatory.</param>
        /// <returns>
        /// <see cref="DocumentDto"/>s object containing the document dto entity.
        /// </returns>
        [HttpPost]
        [DocumentPermissions(Roles = "Manager", ContentOwner = "Own")]       
        public IHttpActionResult Create([FromBody]DocumentViewModel data)
        {
            return PerformAction<DocumentDto>(() =>
            {
                DocumentDto result = null;

                var document = data
                    .ToDto();

                if (document != null)
                {
                    document.UserId = GetCurrentUserId();

                    result = _documentService
                        .Create(document);
                }

                return result;
            });
        }

        /// <summary>
        /// Updates a specific document by identity.
        /// PUT: api/Documents/543D3EC2-BD1F-4AD1-9DAA-D37BE8375893
        /// </summary>
        /// <param name="id">Document identity</param>
        /// <param name="data">Document data. Name field is mandatory.</param>
        /// <returns>
        /// <see cref="DocumentDto"/>s object containing the document dto entity.
        /// </returns>
        [HttpPut]
        [DocumentPermissions(Roles = "Manager", ContentOwner = "Own")]
        public IHttpActionResult Update(Guid id, [FromBody]DocumentViewModel data)
        {
            return PerformAction<DocumentDto>(() =>
            {
                DocumentDto result = null;

                var ctx = GetPermissionsContext();

                var documentDto = _documentService
                    .Get(ctx, id);

                if (documentDto != null)
                {
                    documentDto = data
                        .ToDto(documentDto);

                    result = _documentService
                        .Update(documentDto);
                }

                return result;
            });
        }

        /// <summary>
        /// Delete a specific document by identity.
        /// DELETE: api/Documents/543D3EC2-BD1F-4AD1-9DAA-D37BE8375893 
        /// </summary>
        /// <returns>
        /// True if successfully otherwise False.
        /// </returns>
        [HttpDelete]
        [DocumentPermissions(Roles = "Manager", ContentOwner = "Own")]
        public IHttpActionResult Delete(Guid id)
        {
            return PerformAction<bool>(() =>
            {
                return _documentService
                    .Delete(id);
            });
        }
    }
}