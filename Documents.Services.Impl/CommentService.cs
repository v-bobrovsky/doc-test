using System;
using System.Linq;
using System.Collections.Generic;

using Documents.Data;
using Documents.DataAccess;
using System.Transactions;
using Documents.Utils;

namespace Documents.Services.Impl
{
    /// <summary>
    /// Provides commentaries functionality.
    /// </summary>
    public class CommentService : RepositoryService<CommentDto, int>, ICommentService
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CommentService()
            : base()
        {
        }

        /// <summary>
        /// Validate comment identity
        /// </summary>
        /// <param name="id">Comment identity</param>
        /// <returns></returns>
        protected override bool ValidateEntityKey(int id)
        {
            return (id > 0);
        }

        /// <summary>
        /// Validate new comment entity
        /// </summary>
        /// <param name="entityDto">Comment entity</param>
        /// <returns></returns>
        protected override bool ValidateNewEntity(CommentDto entityDto)
        {
            return entityDto != null
                && !String.IsNullOrEmpty(entityDto.Content);
        }

        /// <summary>
        /// Validate exist comment entity
        /// </summary>
        /// <param name="entityDto">Comment entity</param>
        /// <returns></returns>
        protected override bool ValidateExistEntity(CommentDto entityDto)
        {
            return entityDto != null
                && entityDto.Id > 0
                && !String.IsNullOrEmpty(entityDto.Content);
        }
        
        /// <summary>
        /// Creates a new comment
        /// </summary>
        /// <param name="entityDto">Comment data</param>
        /// <returns>
        /// <see cref="CommentDto"/>s object containing the new comment.
        /// </returns>
        protected override CommentDto OnCreate(CommentDto entityDto)
        {
            CommentDto result = null;

            var entity = entityDto.ToEntity();

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
        /// Retrieves a specific comment
        /// </summary>
        /// <param name="id">Commentary unique identifier</param>
        /// <returns>
        /// <see cref="CommentDto"/>s object containing the new comment.
        /// </returns>
        protected override CommentDto OnGet(int id)
        {
            CommentDto result = null;
            
            var comment = _unitOfWork
                .CommentRepository
                .GetByID(id);

            if (comment != null)
            {
                result = comment
                    .ToDto(_userCtx.GetCurrentId() == comment.UserId);
            }

            return result;
        }

        /// <summary>
        /// Retrieves list of specific comments associated for the document.
        /// </summary>
        /// <param name="args">Document unique identifier</param>
        /// <returns>
        /// List of <see cref="CommentDto"/>s object containing the results in the 
        /// sequence specified in the documentId.
        /// </returns>
        protected override IEnumerable<CommentDto> OnGetAll(params object[] args)
        {
            var isGetCommentsByDocumentId = (args != null
                && args.Length == 1 && args[0] is Guid && !((Guid)args[0]).Equals(Guid.Empty));

            if (isGetCommentsByDocumentId)
                return GetCommentsByDocumentId((Guid)args[0]);
            else 
                return null;
        }

        /// <summary>
        /// Retrieves list of specific comments associated for the document.
        /// </summary>
        /// <param name="documentId">Document unique identifier</param>
        /// <returns>
        /// List of <see cref="CommentDto"/>s object containing the results in the 
        /// sequence specified in the documentId.
        /// </returns>
        protected IEnumerable<CommentDto> GetCommentsByDocumentId(Guid documentId)
        {
            List<CommentDto> result = null;

            var comments = _unitOfWork
                .CommentRepository
                .GetMany(c => documentId.Equals(c.DocumentId))
                .ToList();

            if (comments != null && comments.Any())
            {
                var userId = _userCtx.GetCurrentId();

                result = new List<CommentDto>();
                comments.ForEach(c => result.Add(c.ToDto(userId == c.UserId)));
            }

            return result;
        }

        /// <summary>
        /// Updates a specific comment
        /// </summary>
        /// <param name="comment">Comment data</param>
        /// <returns>
        /// <see cref="CommentDto"/>s object containing the new comment.
        /// </returns>
        protected override CommentDto OnUpdate(CommentDto entityDto)
        {
            CommentDto result = null;

            var oldEntity = _unitOfWork
                .CommentRepository
                .GetByID(entityDto.Id);

            var entity = (oldEntity != null)
                ? entityDto.ToEntity(oldEntity)
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

                result = entity.ToDto(true);
            }

            return result;
        }

        /// <summary>
        /// Delete a specific comment
        /// </summary>
        /// <param name="id">Commentary unique identifier</param>
        /// Return True if succeeded otherwise False
        protected override bool OnDelete(int id)
        {
            using (var scope = new TransactionScope())
            {
                _unitOfWork
                    .CommentRepository
                    .Delete(id);

                _unitOfWork.Save();
                scope.Complete();
            }

            return true;
        }

        /// <summary>
        /// Check is document owner
        /// </summary>
        /// <param name="id">Document unique identifier</param>
        /// <returns></returns>
        public bool CheckIsCommentOwner(int id)
        {
            var comment = Get(id);
            return (comment != null && comment.CanModify);
        }
    }
}