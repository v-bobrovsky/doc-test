using System;
using System.Linq;
using System.Collections.Generic;

using Documents.Data;
using Documents.DataAccess;
using System.Transactions;

namespace Documents.Services.Impl
{
    /// <summary>
    /// Provides commentaries functionality.
    /// </summary>
    public class CommentService : ICommentService
    {
        private readonly UnitOfWork _unitOfWork;

        /// <summary>
        /// Constructor
        /// </summary>
        public CommentService()
        {
            _unitOfWork = new UnitOfWork();
        }

        /// <summary>
        /// Retrieves a specific comment
        /// </summary>
        /// <param name="commentId">Commentary unique identifier</param>
        /// <returns>
        /// <see cref="CommentDto"/>s object containing the new comment.
        /// </returns>
        public CommentDto GetCommentById(int commentId, int userId)
        {
            CommentDto result = null;

            var comment = _unitOfWork
                .CommentRepository
                .GetByID(commentId);

            if (comment != null)
            {
                result = comment
                    .ToDto(userId == comment.UserId);
            }

            return result;
        }

        /// <summary>
        /// Retrieves list of specific comments associated for the document.
        /// </summary>
        /// <param name="documentId">Document unique identifier</param>
        /// <returns>
        /// List of <see cref="CommentDto"/>s object containing the results in the 
        /// sequence specified in the documentId.
        /// </returns>
        public IEnumerable<CommentDto> GetCommentsByDocumentId(Guid documentId, int userId)
        {
            List<CommentDto> result = null;

            var comments = _unitOfWork
                .CommentRepository
                .GetMany(c => documentId.Equals(c.DocumentId))
                .ToList();

            if (comments != null && comments.Any())
            {
                result = new List<CommentDto>();
                comments.ForEach(c => result.Add(c.ToDto(userId == c.UserId)));
            }

            return result;
        }

        /// <summary>
        /// Creates a new comment
        /// </summary>
        /// <param name="comment">Comment data</param>
        /// <returns>
        /// <see cref="CommentDto"/>s object containing the new comment.
        /// </returns>
        public CommentDto CreateComment(CommentDto comment, int userId)
        {
            CommentDto result = null;

            var entity = comment.ToEntity(userId);

            if (entity != null)
            {
                using (var scope = new TransactionScope())
                {
                    _unitOfWork
                        .CommentRepository
                        .Insert(entity);

                    _unitOfWork.Save();
                    scope.Complete();
                }

                result = entity.ToDto(true);
            }

            return result;
        }

        /// <summary>
        /// Updates a specific comment
        /// </summary>
        /// <param name="comment">Comment data</param>
        /// <returns>
        /// Return True if succeeded otherwise False
        /// </returns>
        public bool UpdateComment(CommentDto comment)
        {
            bool result = false;

            if (comment != null && comment.Id > 0)
            {
                var oldEntity = _unitOfWork
                    .CommentRepository
                    .GetByID(comment.Id);

                var entity = (oldEntity != null)
                    ? comment.ToEntity(oldEntity)
                    : null;

                if (entity != null)
                {
                    using (var scope = new TransactionScope())
                    {
                        _unitOfWork
                            .CommentRepository
                            .Update(entity);

                        _unitOfWork.Save();
                        scope.Complete();
                    }

                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Delete a specific comment
        /// </summary>
        /// <param name="commentId">Commentary unique identifier</param>
        /// Return True if succeeded otherwise False
        public bool DeleteComment(int commentId)
        {
            bool result = false;

            if (commentId > 0)
            {
                using (var scope = new TransactionScope())
                {
                    _unitOfWork
                        .CommentRepository
                        .Delete(commentId);

                    _unitOfWork.Save();
                    scope.Complete();
                }

                result = true;
            }

            return result;
        }

        /// <summary>
        /// Check is document owner
        /// </summary>
        /// <param name="documentId">Document unique identifier</param>
        /// <param name="userId">Owner user id</param>
        /// <returns></returns>
        public bool CheckIsCommentOwner(int commentId, int userId)
        {
            var comment = GetCommentById(commentId, userId);
            return (comment != null && comment.CanModify);
        }
    }
}