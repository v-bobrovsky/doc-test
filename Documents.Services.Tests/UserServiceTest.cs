using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Documents.Data;
using Documents.Common;
using System.Transactions;

namespace Documents.Services.Tests
{
    [TestClass]
    public class UserServiceTest
        : ServiceTestBase<UserDto, int>
    {
        #region Constants

        private static readonly string _testUserLogin = "test1@test.com";
        private static readonly string _testUserName = "Test User 1";
        private static readonly string _testUserPassword = "12345";
        private static readonly string _testUserRole = "12345";

        private static readonly string _documentName = "Test document";
        private static readonly string _documentContent = "'Lorem ipsum' dolor sit amet, <br /> consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

        private static readonly string _commentSubj = "Test comment";
        private static readonly string _commentContent = "'Lorem ipsum' dolor sit amet, <br /> consectetur adipiscing elit.";

        #endregion

        /// <summary>
        /// Constructors
        /// </summary>
        public UserServiceTest()
            : base()
        {
            _testService = ObjectContainer.Resolve<IUserService>();
        }

        [TestMethod]
        public void SaveUser_Successfully()
        {
            SaveEntity_SuccessfullySaved();
        }

        [TestMethod]
        public void SaveUser_Error()
        {
            SaveEntity_ErrorSaved();
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException), "Number of new items not equal loaded items.")]
        public void RetrieveUser_Successfully()
        {
            LoadEntities_SuccessfullyLoaded();
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException), "Children has exists after that parent was deleted")]
        public void DeleteUser_Successfully()
        {
            DeleteEntity_SuccessfullyDeleted();
        }

        protected DocumentDto GetDocument(UserDto user)
        {
            return new DocumentDto()
            {
                UserName = user.UserName,
                UserId = user.Id,
                Name = _documentName,
                Content = _documentContent
            };
        }

        protected IEnumerable<CommentDto> GetComments(UserDto user, DocumentDto document)
        {
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

        protected override int GetIdentity(UserDto entityDto)
        {
            return entityDto != null ? entityDto.Id : 0;
        }

        protected override UserDto PrepareValidEntity()
        {
            return new UserDto()
            {
                CreatedTime = DateTime.Now,
                ModifiedTime = DateTime.Now,
                UserName = _testUserName,
                Login = _testUserLogin,
                Password = _testUserPassword,
                UserRole = _testUserRole
            };
        }

        protected override UserDto PrepareInvalidEntity()
        {
            return new UserDto()
            {
                CreatedTime = DateTime.Now,
                ModifiedTime = DateTime.Now,
                UserRole = _testUserRole
            };
        }

        protected override IEnumerable<UserDto> PrepareValidEntities()
        {
            return new List<UserDto>()
            {
                new UserDto()
                {
                    CreatedTime = DateTime.Now,
                    ModifiedTime = DateTime.Now,
                    UserName = _testUserName,
                    Login = _testUserLogin,
                    Password = _testUserPassword,
                    UserRole = _testUserRole
                },

                new UserDto()
                {
                    CreatedTime = DateTime.Now,
                    ModifiedTime = DateTime.Now,
                    UserName = _testUserName + "2",
                    Login = _testUserLogin + "2",
                    Password = _testUserPassword,
                    UserRole = _testUserRole
                }
            };
        }

        /// <summary>
        /// Validate has user exists the documents and comments
        /// </summary>
        /// <param name="parentId">Parent entity identity</param>
        /// <returns></returns>
        protected override bool CheckChildrenExists(int parentId)
        {
            var documents = _unitOfWork
                     .DocumentRepository
                     .GetManyQueryable(p => p.UserId == parentId)
                     .ToList();

            var comments = _unitOfWork
                     .CommentRepository
                     .GetManyQueryable(p=> p.UserId == parentId)
                     .ToList();

            return ((comments != null && comments.Any()) 
                || (documents != null && documents.Any()));
        }

        protected override UserDto PrepareAndCreateValidEntityWithChildren()
        {
            var user = PrepareValidEntity();
            var userEntity = user.ToEntity();

            using (var scope = new TransactionScope())
            {
                _unitOfWork
                     .UserRepository
                     .Insert(userEntity);

                _unitOfWork.Save();
                scope.Complete();
            }

            user = userEntity.ToDto();

            var document = GetDocument(user);
            var documentEntity = document.ToEntity();

            using (var scope = new TransactionScope())
            {
                _unitOfWork
                     .DocumentRepository
                     .Insert(documentEntity);

                _unitOfWork.Save();
                scope.Complete();
            }

            documentEntity = _unitOfWork
                .DocumentRepository
                .GetWithInclude(p => p.Id.Equals(documentEntity.Id), "User")
                .FirstOrDefault();

            document = documentEntity.ToDto(true);

            var comments = GetComments(user, document);

            using (var scope = new TransactionScope())
            {
                foreach (var comment in comments)
                {
                    _unitOfWork
                        .CommentRepository
                        .Insert(comment.ToEntity());
                }

                _unitOfWork.Save();
                scope.Complete();
            }

            return user;
        }
    }
}