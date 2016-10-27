using System;
using System.Collections.Generic;

using Documents.Data;

namespace Documents.Services
{
    /// <summary>
    /// Provides documents functionality.
    /// </summary>
    public interface IDocumentService
    {
        /// <summary>
        /// Retrieves a specific comment
        /// </summary>
        /// <param name="documentId">Document unique identifier</param>
        /// <returns></returns>
        DocumentDto GetDocumentById(Guid documentId, int userId);

        /// <summary>
        /// Retrieves list of all specific documents.
        /// </summary>
        /// <param name="documentId">Document unique identifier</param>
        /// <returns>
        /// List of <see cref="DocumentDto"/>s object containing the results.
        /// </returns>
        IEnumerable<DocumentDto> GetAllDocuments(int userId);

        /// <summary>
        /// Retrieves list of specific documents associated for the user.
        /// </summary>
        /// <param name="userId">User unique identifier</param>
        /// <returns>
        /// List of <see cref="DocumentDto"/>s object containing the results in the 
        /// sequence specified in the userId.
        /// </returns>
        IEnumerable<DocumentDto> GetDocumentsByUserId(int userId);

        /// <summary>
        /// Creates a new document
        /// </summary>
        /// <param name="document">Document data</param>
        /// <returns>
        /// <see cref="DocumentDto"/>s object containing the new document.
        /// </returns>
        DocumentDto CreateDocument(DocumentDto document, int userId);

        /// <summary>
        /// Updates a specific document
        /// </summary>
        /// <param name="document">Document data</param>
        /// <returns>
        /// Return True if succeeded otherwise False
        /// </returns>
        bool UpdateDocument(DocumentDto document);

        /// <summary>
        /// Delete a specific document
        /// </summary>
        /// <param name="documentId">Document unique identifier</param>
        /// Return True if succeeded otherwise False
        bool DeleteDocument(Guid documentId);

        /// <summary>
        /// Check is document owner
        /// </summary>
        /// <param name="documentId">Document unique identifier</param>
        /// <param name="userId">Owner user id</param>
        /// <returns></returns>
        bool CheckIsDocumentOwner(Guid documentId, int userId);
    }
}