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
    /// <summary>
    /// Provides CRUD functionality for comments.
    /// </summary>
    [Authorize]
    public class CommentsController : BaseController
    {
        private readonly IUserContext _userContext;
        private readonly ICommentService _commentService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userContext"></param>
        /// <param name="commentService"></param>
        public CommentsController(IUserContext userContext, ICommentService commentService)
        {
            _userContext = userContext;
            _commentService = commentService;
        }

        /// <summary>
        /// Retrieves list of comments assosiated with document.
        /// GET: api/Comments/?documentId=543D3EC2-BD1F-4AD1-9DAA-D37BE8375893
        /// </summary>
        /// <param name="id">Document identity</param>
        /// <returns>
        /// <see cref="IEnumerable&lt;CommentDto&gt;"/>s object containing the list of comments.
        /// </returns>
        [HttpGet]
        public IHttpActionResult Retrive(Guid documentId)
        {
            return PerformAction<IEnumerable<CommentDto>>(() =>
            {
                return _commentService
                    .GetAll(documentId);
            });
        }

        /// <summary>
        /// Retrives a specific comment by identity.
        /// GET: api/Comments/5
        /// </summary>
        /// <param name="id">Comment identity</param>
        /// <returns>
        /// <see cref="CommentDto"/>s object containing the comment dto entity.
        /// </returns>
        [HttpGet]
        public IHttpActionResult Retrive(int id)
        {
            return PerformAction<CommentDto>(() =>
            {
                return _commentService
                    .Get(id);
            });
        }

        /// <summary>
        /// Creates a new comment.
        /// POST: api/Comments
        /// </summary>
        /// <param name="data">Comment data. Name field is mandatory.</param>
        /// <returns>
        /// <see cref="CommentDto"/>s object containing the comment dto entity.
        /// </returns>
        [HttpPost]
        public IHttpActionResult Create([FromBody]CommentViewModel data)
        {
            return PerformAction<CommentDto>(() =>
            {
                CommentDto result = null;

                var comment = data
                    .ToDto();

                if (comment != null)
                {
                    comment.UserId  = _userContext
                        .GetCurrentId();

                    result = _commentService
                        .Create(comment);
                }

                return result;
            });
        }

        /// <summary>
        /// Updates a specific comment by identity.
        /// PUT: api/Comments/5
        /// </summary>
        /// <param name="id">Comment identity</param>
        /// <param name="data">Comment data. Text field is mandatory.</param>
        /// <returns>
        /// <see cref="CommentDto"/>s object containing the comment dto entity.
        /// </returns>
        [HttpPut]
        [CommentPermissions(ContentOwner = "Own")]
        public IHttpActionResult Update(int id, [FromBody]CommentViewModel data)
        {
            return PerformAction<CommentDto>(() =>
            {
                CommentDto result = null;

                var commentDto = _commentService
                    .Get(id);

                if (commentDto != null)
                {
                    commentDto = data
                        .ToDto(commentDto);

                    result = _commentService
                        .Update(commentDto);
                }

                return result;
            });
        }

        /// <summary>
        /// Delete a specific comment by identity.
        /// DELETE: api/Comments/5
        /// </summary>
        /// <param name="id">Comment identity</param>
        /// <returns>
        /// True if successfully otherwise False.
        /// </returns>
        [HttpDelete]
        [CommentPermissions(ContentOwner = "Own")]
        public IHttpActionResult Delete(int id)
        {
            return PerformAction<bool>(() =>
            {
                return _commentService
                    .Delete(id);
            });
        }
    }
}