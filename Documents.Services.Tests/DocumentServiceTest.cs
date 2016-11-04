using System;
using System.Linq;
using System.Transactions;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Documents.Data;
using Documents.Common;
using Documents.Services.Tests.Core;
using Documents.DataAccess;

namespace Documents.Services.Tests
{
    [TestClass]
    public class DocumentServiceTest
        : ServiceTestBase<DocumentDto, Guid>
    {
        #region Constants

        private static readonly string _documentName = "Test document";
        private static readonly string _documentContent = "'Lorem ipsum' dolor sit amet, <br /> consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

        private static readonly string _commentSubj = "Test comment";
        private static readonly string _commentContent = "'Lorem ipsum' dolor sit amet, <br /> consectetur adipiscing elit.";

        #endregion

        /// <summary>
        /// Constructors
        /// </summary>
        public DocumentServiceTest()
            : base()
        {
            _testService = ObjectContainer.Resolve<IDocumentService>();
        }

        [TestMethod]
        public void SaveDocument_Successfully()
        {           
            SaveEntity_SuccessfullySaved();
        }

        [TestMethod]
        public void SaveDocument_Error()
        {
            SaveEntity_ErrorSaved();
        }

        [TestMethod]
        public void RetrieveDocuments_Successfully()
        {
            LoadEntities_SuccessfullyLoaded();
        }

        [TestMethod]
        public void DeleteDocument_Successfully()
        {
            DeleteEntity_SuccessfullyDeleted();
        }

        protected IEnumerable<CommentDto> GetComments(DocumentDto document)
        {
            var user = GetTestUser();

            return new List<CommentDto>()
            {
                new CommentDto()
                {
                    DocumentId = document.Id,
                    UserName = user.UserName,
                    UserId = user.Id,
                    Subject = _commentSubj,
                    Content = _commentContent
                },

                new CommentDto()
                {
                    DocumentId = document.Id,
                    UserName = user.UserName,
                    UserId = user.Id,
                    Content = _commentContent
                }
            };
        }

        protected override Guid GetIdentity(DocumentDto entityDto)
        {
            return entityDto != null ? entityDto.Id : Guid.Empty;
        }

        protected override DocumentDto PrepareValidEntity()
        {
            var user = GetTestUser();

            return new DocumentDto()
            {
                UserName = user.UserName,
                UserId = user.Id,
                Name = _documentName,
                Content = _documentContent
            };
        }

        protected override DocumentDto PrepareInvalidEntity()
        {
            return new DocumentDto()
            {
                UserId = 0,
                Name = string.Empty,
                Content = string.Empty
            };
        }

        protected override IEnumerable<DocumentDto> PrepareValidEntities()
        {
            var user = GetTestUser();

            return new List<DocumentDto>()
            {
                new DocumentDto()
                {
                    UserName = user.UserName,
                    UserId = user.Id,
                    Name = _documentName,
                    Content = _documentContent
                },

                new DocumentDto()
                {
                    UserId = user.Id,
                    Name = _documentName + " second",
                    Content = _documentContent
                }
            };
        }

        /// <summary>
        /// Validate has document exists the comments
        /// </summary>
        /// <param name="parentId">Parent entity identity</param>
        /// <returns></returns>
        protected override bool CheckChildrenExists(Guid parentId)
        {
            using (var unitOfWork = ObjectContainer.Resolve<UnitOfWork>())
            {
                var comments = unitOfWork
                         .CommentRepository
                         .GetManyQueryable(p => p.DocumentId.Equals(parentId))
                         .ToList();

                return (comments != null && comments.Any());
            }
        }

        protected override DocumentDto PrepareAndCreateValidEntityWithChildren()
        {
            var document = PrepareValidEntity();

            var entity = document
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

                document = entity.ToDto(true);

                var comments = GetComments(document);

                using (var scope = new TransactionScope())
                {
                    foreach (var comment in comments)
                    {
                        unitOfWork
                            .CommentRepository
                            .Insert(comment.ToEntity());
                    }

                    unitOfWork
                        .Save();
                    scope.Complete();
                }
            }

            return document;
        }
    }
}