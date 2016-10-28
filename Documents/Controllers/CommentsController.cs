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
                return _commentService.GetAll(documentId);
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
                return _commentService.Get(id);
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
                return _commentService.Create(comment);
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
            return PerformAction<CommentDto>(() =>
            {
                var comment = data.ToDto();
                comment.Id = id;
                return _commentService.Create(comment);
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
                return _commentService.Delete(id);
            });
        }
    }
}