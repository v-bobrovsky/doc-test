using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Documents.Data;
using Documents.Common;
using System.Transactions;
using Documents.Services.Tests.Core;

namespace Documents.Services.Tests
{
    [TestClass]
    public class CommentServiceTest
        : ServiceTestBase<CommentDto, int>
    {
        #region Constants

        private static readonly string _documentName = "Test document";
        private static readonly string _documentContent = "'Lorem ipsum' dolor sit amet, <br /> consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

        private static readonly string _commentSubj = "Test comment";
        private static readonly string _commentContent = "'Lorem ipsum' dolor sit amet, <br /> consectetur adipiscing elit.";

        #endregion

        #region Members

        private Lazy<DocumentDto> _document;

        #endregion

        /// <summary>
        /// Constructors
        /// </summary>
        public CommentServiceTest()
            : base()
        {
            _testService = ObjectContainer.Resolve<ICommentService>();
        }

        [TestMethod]
        public void SaveComment_Successfully()
        {
            SaveEntity_SuccessfullySaved();
        }

        [TestMethod]
        public void SaveComment_Error()
        {
            SaveEntity_ErrorSaved();
        }

        [TestMethod]
        public void RetrieveComments_Successfully()
        {
            LoadEntities_SuccessfullyLoaded();
        }

        [TestMethod]
        public void DeleteComment_Successfully()
        {
            DeleteEntity_SuccessfullyDeleted();
        }

        /// <summary>
        /// Create a test user
        /// </summary>
        /// <returns></returns>
        private DocumentDto CreateDocument()
        {
            var user = GetTestUser();

            var document = new DocumentDto()
            {
                UserName = user.UserName,
                UserId = user.Id,
                CreatedTime = DateTime.Now,
                ModifiedTime = DateTime.Now,
                Name = _documentName,
                Content = _documentContent
            };

            var entity = document.ToEntity();

            using (var scope = new TransactionScope())
            {
                _unitOfWork
                     .DocumentRepository
                     .Insert(entity);

                _unitOfWork.Save();
                scope.Complete();
            }

            return entity.ToDto(true);
        }

        /// <summary>
        /// Retrieves a document
        /// </summary>
        /// <returns></returns>
        protected DocumentDto GetDocument()
        {
            if (_document == null)
            {
                _document = new Lazy<DocumentDto>(() =>
                {
                    return CreateDocument();
                });
            }

            return _document.Value;
        }

        protected override List<object> GetRetriveDataVariantKeys()
        {
            var document = GetDocument();

            return new List<object>()
            {
                document.Id
            };
        }

        protected override int GetIdentity(CommentDto entityDto)
        {
            return entityDto != null ? entityDto.Id : 0;
        }

        protected override CommentDto PrepareValidEntity()
        {
            var user = GetTestUser();
            var document = GetDocument();

            return new CommentDto()
            {
                DocumentId = document.Id,
                UserName = user.UserName,
                UserId = user.Id,
                Subject = _commentSubj,
                Content = _commentContent
            };
        }

        protected override CommentDto PrepareInvalidEntity()
        {
            return new CommentDto()
            {
                DocumentId = Guid.Empty,
                UserId = 0,
                Subject = _commentSubj,
                Content = string.Empty
            };
        }

        protected override IEnumerable<CommentDto> PrepareValidEntities()
        {
            var user = GetTestUser();
            var document = GetDocument();

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

        protected override CommentDto PrepareAndCreateValidEntityWithChildren()
        {
            var comment = PrepareValidEntity();

            var entity = comment.ToEntity();

            using (var scope = new TransactionScope())
            {
                _unitOfWork
                     .CommentRepository
                     .Insert(entity);

                _unitOfWork.Save();
                scope.Complete();
            }

            entity = _unitOfWork
                .CommentRepository
                .GetWithInclude(p => p.Id.Equals(entity.Id), "User")
                .FirstOrDefault();

            comment = entity.ToDto(true);

            return comment;
        }
    }
}