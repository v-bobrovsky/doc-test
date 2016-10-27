using System;
using System.Web.Http;
using System.Collections.Generic;

using Documents.Data;
using Documents.Core;
using Documents.Models;
using Documents.Services;
using Documents.Filters;

namespace Documents.Controllers
{
    public class CommentsController : BaseController
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        /// <summary>
        /// GET: api/Comments
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        public IHttpActionResult Get(Guid documentId)
        {
            return PerformAction<IEnumerable<CommentDto>>(() =>
            {
                return _commentService.GetCommentsByDocumentId(documentId, 0);
            });
        }

        /// <summary>
        /// GET: api/Comments/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult Get(int id)
        {
            return PerformAction<CommentDto>(() =>
            {
                return _commentService.GetCommentById(id, 0);
            });
        }

        /// <summary>
        /// POST: api/Comments
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IHttpActionResult Post([FromBody]CommentViewModel data)
        {
            return PerformAction<CommentDto>(() =>
            {
                var comment = data.ToDto();
                return _commentService.CreateComment(comment, 0);
            });
        }

        /// <summary>
        /// PUT: api/Comments/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [CommentPermissions(ContentOwner = "Own")]
        public IHttpActionResult Put(int id, [FromBody]CommentViewModel data)
        {
            return PerformAction<bool>(() =>
            {
                var comment = _commentService.GetCommentById(id, 0);
                comment = data.ToDto(comment);
                return _commentService.UpdateComment(comment);
            });
        }

        /// <summary>
        /// DELETE: api/Comments/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CommentPermissions(ContentOwner = "Own")]
        public IHttpActionResult Delete(int id)
        {
            return PerformAction<bool>(() =>
            {
                return _commentService.DeleteComment(id);
            });
        }
    }
}