using System;
using System.Linq;
using System.Collections.Generic;

using Documents.Data;
using Documents.DataAccess;
using System.Transactions;

namespace Documents.Services.Impl
{
    /// <summary>
    /// Provides documents functionality.
    /// </summary>
    public class DocumentService : IDocumentService
    {
        private readonly UnitOfWork _unitOfWork;

        /// <summary>
        /// Constructor
        /// </summary>
        public DocumentService()
        {
            _unitOfWork = new UnitOfWork();
        }

        /// <summary>
        /// Retrieves a specific comment
        /// </summary>
        /// <param name="documentId">Document unique identifier</param>
        /// <returns></returns>
        public DocumentDto GetDocumentById(Guid documentId, int userId)
        {
            DocumentDto result = null;

            var document = _unitOfWork
                .DocumentRepository
                .GetByID(documentId);

            if (document != null)
            {
                result = document
                    .ToDto(userId == document.UserId);
            }

            return result;
        }

        /// <summary>
        /// Retrieves list of all specific documents.
        /// </summary>
        /// <param name="documentId">Document unique identifier</param>
        /// <returns>
        /// List of <see cref="DocumentDto"/>s object containing the results.
        /// </returns>
        public IEnumerable<DocumentDto> GetAllDocuments(int userId)
        {
            List<DocumentDto> result = null;

            var documents = _unitOfWork
                .DocumentRepository
                .GetAll()
                .ToList();

            if (documents != null && documents.Any())
            {
                result = new List<DocumentDto>();
                documents.ForEach(d => result.Add(d.ToDto(userId == d.UserId)));
            }

            return result;
        }

        /// <summary>
        /// Retrieves list of specific documents associated for the user.
        /// </summary>
        /// <param name="userId">User unique identifier</param>
        /// <returns>
        /// List of <see cref="DocumentDto"/>s object containing the results in the 
        /// sequence specified in the userId.
        /// </returns>
        public IEnumerable<DocumentDto> GetDocumentsByUserId(int userId)
        {
            List<DocumentDto> result = null;

            var documents = _unitOfWork
                .DocumentRepository
                .GetMany(d => d.UserId == userId)
                .ToList();

            if (documents != null && documents.Any())
            {
                result = new List<DocumentDto>();
                documents.ForEach(c => result.Add(c.ToDto(userId == c.UserId)));
            }

            return result;
        }

        /// <summary>
        /// Creates a new document
        /// </summary>
        /// <param name="document">Document data</param>
        /// <returns>
        /// <see cref="DocumentDto"/>s object containing the new document.
        /// </returns>
        public DocumentDto CreateDocument(DocumentDto document, int userId)
        {
            DocumentDto result = null;

            var entity = document.ToEntity(userId);

            if (entity != null)
            {
                using (var scope = new TransactionScope())
                {
                    _unitOfWork
                        .DocumentRepository
                        .Insert(entity);

                    _unitOfWork.Save();
                    scope.Complete();
                }

                result = entity.ToDto(true);
            }

            return result;
        }

        /// <summary>
        /// Updates a specific document
        /// </summary>
        /// <param name="document">Document data</param>
        /// <returns>
        /// Return True if succeeded otherwise False
        /// </returns>
        public bool UpdateDocument(DocumentDto document)
        {
            bool result = false;

            if (document != null && !document.Id.Equals(Guid.Empty))
            {
                var oldEntity = _unitOfWork
                    .DocumentRepository
                    .GetByID(document.Id);

                var entity = (oldEntity != null)
                    ? document.ToEntity(oldEntity) 
                    : null;

                if (entity != null)
                {
                    using (var scope = new TransactionScope())
                    {
                        _unitOfWork
                            .DocumentRepository
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
        /// Delete a specific document
        /// </summary>
        /// <param name="documentId">Document unique identifier</param>
        /// Return True if succeeded otherwise False
        public bool DeleteDocument(Guid documentId)
        {
            bool result = false;

            if (!documentId.Equals(Guid.Empty))
            {
                using (var scope = new TransactionScope())
                {
                    _unitOfWork
                        .DocumentRepository
                        .Delete(documentId);

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
        public bool CheckIsDocumentOwner(Guid documentId, int userId)
        {
            var document = GetDocumentById(documentId, userId);
            return (document != null && document.CanModify);
        }
    }
}
