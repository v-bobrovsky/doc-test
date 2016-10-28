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
    public class DocumentsController : BaseController
    {
        private readonly IDocumentService _documentService;

        public DocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        /// <summary>
        /// GET: api/Documents
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult Get()
        {
            return PerformAction<IEnumerable<DocumentDto>>(() =>
            {
                return _documentService.GetAll();
            });
        }

        /// <summary>
        /// GET: api/Documents/543D3EC2-BD1F-4AD1-9DAA-D37BE8375893
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult Get(Guid id)
        {
            return PerformAction<DocumentDto>(() =>
            {
                return _documentService.Get(id);
            });
        }

        /// <summary>
        /// POST: api/Documents
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [DocumentPermissions(Roles = "Manager")]
        public IHttpActionResult Post([FromBody]DocumentViewModel data)
        {
            return PerformAction<DocumentDto>(() =>
            {
                var document = data.ToDto();
                return _documentService.Create(document);
            });
        }

        /// <summary>
        /// PUT: api/Documents/543D3EC2-BD1F-4AD1-9DAA-D37BE8375893
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [DocumentPermissions(Roles = "Manager", ContentOwner = "Own")]
        public IHttpActionResult Put(Guid id, [FromBody]DocumentViewModel data)
        {
            return PerformAction<DocumentDto>(() =>
            {
                var document = data.ToDto();
                document.Id = id;

                return _documentService.Create(document);
            });
        }

        /// <summary>
        /// DELETE: api/Documents/543D3EC2-BD1F-4AD1-9DAA-D37BE8375893 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [DocumentPermissions(Roles = "Manager", ContentOwner = "Own")]
        public IHttpActionResult Delete(Guid id)
        {
            return PerformAction<bool>(() =>
            {
                return _documentService.Delete(id);
            });
        }
    }
}