using System;
using System.Linq;
using System.Transactions;
using System.Collections.Generic;

using Documents.Data;
using Documents.Common;
using Documents.DataAccess;
using Documents.Utils;

namespace Documents.Services.Impl
{
    /// <summary>
    /// Provides documents functionality.
    /// </summary>
    public class DocumentService : RepositoryService<DocumentDto, Guid>, IDocumentService
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DocumentService()
            : base()
        {
        }

        /// <summary>
        /// Validate document identity
        /// </summary>
        /// <param name="id">Document identity</param>
        /// <returns></returns>
        protected override bool ValidateEntityKey(Guid id)
        {
            return !id.Equals(Guid.Empty);
        }

        /// <summary>
        /// Validate new document entity
        /// </summary>
        /// <param name="entityDto">Document entity</param>
        /// <returns></returns>
        protected override bool ValidateNewEntity(DocumentDto entityDto)
        {
            return entityDto != null
                && entityDto.UserId > 0
                && !String.IsNullOrEmpty(entityDto.Name);
        }

        /// <summary>
        /// Validate exist document entity
        /// </summary>
        /// <param name="entityDto">Document entity</param>
        /// <returns></returns>
        protected override bool ValidateExistEntity(DocumentDto entityDto)
        {
            return entityDto != null
                && !entityDto.Id.Equals(Guid.Empty)
                && entityDto.UserId > 0
                && !String.IsNullOrEmpty(entityDto.Name);
        }

        /// <summary>
        /// Creates a new document
        /// </summary>
        /// <param name="entity">Document data</param>
        /// <returns>
        /// <see cref="DocumentDto"/>s object containing the new document.
        /// </returns>
        protected override DocumentDto OnCreate(DocumentDto entityDto)
        {
            var entity = entityDto
                .ToEntity();

            using (var unitOfWork = ObjectContainer.Resolve<UnitOfWork>())
            {
                using (var scope = new TransactionScope())
                {
                    unitOfWork
                        .DocumentRepository
                        .Insert(entity);

                    unitOfWork
                        .Save();
                    scope.Complete();
                }

                entity = unitOfWork
                    .DocumentRepository
                    .GetWithInclude(p => p.Id.Equals(entity.Id), "User")
                    .FirstOrDefault();
            }

            return entity
                .ToDto(true);
        }

        /// <summary>
        /// Retrieves a specific document
        /// </summary>
        /// <param name="ctx">Contains information of current user</param>
        /// <param name="documentId">Document unique identifier</param>
        /// <returns></returns>
        protected override DocumentDto OnGet(PermissionsContext ctx, Guid id)
        {
            DocumentDto result = null;

            using (var unitOfWork = ObjectContainer.Resolve<UnitOfWork>())
            {
                var document = unitOfWork
                    .DocumentRepository
                    .GetWithInclude(p => p.Id.Equals(id), "User")
                    .FirstOrDefault();

                if (document != null)
                {
                    var userId = ctx != null
                        ? ctx.CurrentUserId : 0;

                    result = document
                        .ToDto(userId == document.UserId);
                }
            }

            return result;
        }

        /// <summary>
        /// Retrieves list of all specific documents. 
        /// Or retrieves list of specific documents associated for the user.
        /// </summary>
        /// <param name="ctx">Contains information of current user</param>
        /// <returns>
        /// List of <see cref="DocumentDto"/>s object containing the results.
        /// </returns>
        protected override IEnumerable<DocumentDto> OnGetAll(PermissionsContext ctx, params object[] args)
        {
            var isGetDocumentsByUserId = (args != null
                && args.Length == 1 && args[0] is int && (int)args[0] > 0);
            
            if (isGetDocumentsByUserId)
                return GetDocumentsByUserId((int)args[0]);
            else
                return GetAllDocuments(ctx);
        }

        /// <summary>
        /// Retrieves list of all specific documents.
        /// </summary>
        /// <param name="ctx">Contains information of current user</param>
        /// <returns>
        /// List of <see cref="DocumentDto"/>s object containing the results.
        /// </returns>
        protected IEnumerable<DocumentDto> GetAllDocuments(PermissionsContext ctx)
        {
            List<DocumentDto> result = null;

            using (var unitOfWork = ObjectContainer.Resolve<UnitOfWork>())
            {
                var documents = unitOfWork
                    .DocumentRepository
                    .GetWithInclude(p => true, "User")
                    .ToList();

                if (documents != null && documents.Any())
                {
                    var userId = ctx != null
                        ? ctx.CurrentUserId : 0;

                    result = new List<DocumentDto>();
                    documents.ForEach(d => result.Add(d.ToDto(userId == d.UserId)));
                }
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
        protected IEnumerable<DocumentDto> GetDocumentsByUserId(int userId)
        {
            List<DocumentDto> result = null;

            using (var unitOfWork = ObjectContainer.Resolve<UnitOfWork>())
            {
                var documents = unitOfWork
                    .DocumentRepository
                    .GetWithInclude(p => p.UserId == userId, "User")
                    .ToList();

                if (documents != null && documents.Any())
                {
                    result = new List<DocumentDto>();
                    documents.ForEach(d => result.Add(d.ToDto(true)));
                }
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
        protected override DocumentDto OnUpdate(DocumentDto entityDto)
        {
            DocumentDto result = null;

            using (var unitOfWork = ObjectContainer.Resolve<UnitOfWork>())
            {
                var oldEntity = unitOfWork
                    .DocumentRepository
                    .GetByID(entityDto.Id);

                var entity = (oldEntity != null)
                    ? entityDto.ToEntity(oldEntity)
                    : null;

                if (entity != null)
                {
                    using (var scope = new TransactionScope())
                    {
                        unitOfWork
                            .DocumentRepository
                            .Update(entity);

                        unitOfWork
                            .Save();
                        scope.Complete();
                    }

                    entity = unitOfWork
                        .DocumentRepository
                        .GetWithInclude(p => p.Id.Equals(entity.Id), "User")
                        .FirstOrDefault();

                    result = entity.ToDto(true);
                }
            }

            return result;
        }

        /// <summary>
        /// Delete a specific document
        /// </summary>
        /// <param name="documentId">Document unique identifier</param>
        /// Return True if succeeded otherwise False

        protected override bool OnDelete(Guid id)
        {
            using (var unitOfWork = ObjectContainer.Resolve<UnitOfWork>())
            {
                using (var scope = new TransactionScope())
                {
                    unitOfWork
                        .DocumentRepository
                        .Delete(id);

                    unitOfWork
                        .Save();
                    scope.Complete();
                }
            }

            return true;
        }

        /// <summary>
        /// Check is document owner
        /// </summary>
        /// <param name="ctx">Contains information of current user</param>
        /// <param name="documentId">Document unique identifier</param>
        /// <param name="userId">Owner user id</param>
        /// <returns></returns>
        public bool CheckIsDocumentOwner(PermissionsContext ctx, Guid id)
        {
            var document = Get(ctx, id);
            return (document != null && document.CanModify);
        }
    }
}