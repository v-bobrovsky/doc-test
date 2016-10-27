using System;
using System.Collections.Generic;

using Documents.Data;

namespace Documents.Services
{
    /// <summary>
    /// Provides commentaries functionality.
    /// </summary>
    public interface ICommentService
    {
        /// <summary>
        /// Retrieves a specific comment
        /// </summary>
        /// <param name="commentId">Commentary unique identifier</param>
        /// <returns>
        /// <see cref="CommentDto"/>s object containing the new comment.
        /// </returns>
        CommentDto GetCommentById(int commentId, int userId);

        /// <summary>
        /// Retrieves list of specific comments associated for the document.
        /// </summary>
        /// <param name="documentId">Document unique identifier</param>
        /// <returns>
        /// List of <see cref="CommentDto"/>s object containing the results in the 
        /// sequence specified in the documentId.
        /// </returns>
        IEnumerable<CommentDto> GetCommentsByDocumentId(Guid documentId, int userId);

        /// <summary>
        /// Creates a new comment
        /// </summary>
        /// <param name="comment">Comment data</param>
        /// <returns>
        /// <see cref="CommentDto"/>s object containing the new comment.
        /// </returns>
        CommentDto CreateComment(CommentDto comment, int userId);

        /// <summary>
        /// Updates a specific comment
        /// </summary>
        /// <param name="comment">Comment data</param>
        /// <returns>
        /// Return True if succeeded otherwise False
        /// </returns>
        bool UpdateComment(CommentDto comment);

        /// <summary>
        /// Delete a specific comment
        /// </summary>
        /// <param name="commentId">Commentary unique identifier</param>
        /// Return True if succeeded otherwise False
        bool DeleteComment(int commentId);

        /// <summary>
        /// Check is comment owner
        /// </summary>
        /// <param name="commentId">Commentary unique identifier</param>
        /// <param name="userId">Owner user id</param>
        /// <returns></returns>
        bool CheckIsCommentOwner(int commentId, int userId);
    }
}