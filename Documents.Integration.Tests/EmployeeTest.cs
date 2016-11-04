using System;
using System.Linq;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Documents.DataAccess;
using Documents.Integration.Tests.Core;
using System.Threading;

namespace Documents.Integration.Tests
{
    [TestClass]
    public class UserEmployeeTest 
        : ServiceTestBase
    {
        #region Constants

        private static readonly string _testManagerUserLogin = "manager@user.com";
        private static readonly string _testManagerUserName = "Test Manager";
        private static readonly string _testManagerUserPassword = "p1234567!";
        
        private static readonly string _testEmployeeUserLogin = "employee@user.com";
        private static readonly string _testEmployeeUserName = "Test Employee";
        private static readonly string _testEmployeeUserPassword = "p1234567!";

        private static readonly string _documentName = "Test document";
        private static readonly string _documentContent = "'Lorem ipsum' dolor sit amet, <br /> consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

        private static readonly string _commentSubj = "Test comment";
        private static readonly string _commentContent = "'Lorem ipsum' dolor sit amet, <br /> consectetur adipiscing elit.";

        #endregion

        #region Members

        private static int _managerUserId = 0;
        private static int _managerCommentId = 0;
        private static int _employeeCommentId = 0;
        private static Guid _managerDocumentId = Guid.Empty;

        #endregion


        /// <summary>
        /// Constructor
        /// </summary>
        public UserEmployeeTest()
            : base()
        {
        }

        private void SetupTestData()
        {
            DatabaseHelper
                .CleanupForUser(_testManagerUserLogin);

            DatabaseHelper
                .CleanupForUser(_testEmployeeUserLogin);

            using (var unitOfWork = ObjectContainer.Resolve<UnitOfWork>())
            {
                //Add manager user
                var managerUser = new User()
                {
                    CreatedTime = DateTime.Now,
                    ModifiedTime = DateTime.Now,
                    Login = _testManagerUserLogin,
                    UserName = _testManagerUserName,
                    Password = _testManagerUserPassword,
                    Role = Roles.Manager
                };

                using (var scope = new TransactionScope())
                {
                    unitOfWork
                        .UserRepository
                        .Insert(managerUser);

                    unitOfWork
                        .Save();
                    scope.Complete();
                }

                _managerUserId = managerUser.UserId;

                //Add new document from manager
                var document = new Document()
                {
                    Id = System.Guid.NewGuid(),
                    CreatedTime = DateTime.Now,
                    ModifiedTime = DateTime.Now,
                    Name = _documentName,
                    Content = _documentContent,
                    UserId = _managerUserId
                };

                using (var scope = new TransactionScope())
                {
                    unitOfWork
                         .DocumentRepository
                         .Insert(document);

                    unitOfWork
                        .Save();
                    scope.Complete();
                }

                _managerDocumentId = document.Id;

                //Add new comment from manager
                var comment = new Comment()
                {
                    CreatedTime = DateTime.Now,
                    ModifiedTime = DateTime.Now,
                    DocumentId = _managerDocumentId,
                    UserId = _managerUserId,
                    Subject = _commentSubj,
                    Content = _commentContent
                };

                using (var scope = new TransactionScope())
                {
                    unitOfWork
                        .CommentRepository
                        .Insert(comment);

                    unitOfWork.Save();
                    scope.Complete();
                }

                _managerCommentId = comment.Id;
            }
        }

        [TestMethod]
        public void T00_RegisterEmployee_Successfully()
        {
            SetupTestData();

            LogoutUser();

            Thread
                .Sleep(1000);

            RegisterUser(_testEmployeeUserName,
                _testEmployeeUserLogin,
                _testEmployeeUserPassword,
                false);

            var userDto = RetriveUser();

            Assert.IsNotNull(userDto);
            Assert.AreEqual(userDto.Login, _testEmployeeUserLogin);
        }

        [TestMethod]
        public void T01_UpdateEmployee_Successfully()
        {
            var userDto = RetriveUser();

            Assert.IsNotNull(userDto);
            Assert.AreEqual(userDto.Login, _testEmployeeUserLogin);

            var newUserName = userDto.UserName + " !";
            var newUserPassword = _testEmployeeUserPassword + "!";

            var editUserDto = EditUser(newUserName, 
                userDto.Login, 
                newUserPassword);

            Assert.IsNotNull(editUserDto);
            Assert.AreEqual(editUserDto.UserName, newUserName);

            LogoutUser();

            Thread
                .Sleep(1000);

            LoginUser(_testEmployeeUserLogin, 
                newUserPassword);

            userDto = RetriveUser();

            Assert.IsNotNull(userDto);
            Assert.AreEqual(userDto.UserName, newUserName);
        }

        [TestMethod]
        public void T02_SaveEmployeeComment_Successfully()
        {
            var commmentDto = AddComment(_managerDocumentId, 
                _commentSubj, 
                _commentContent);

            Assert.IsNotNull(commmentDto);
            Assert.AreEqual(commmentDto.DocumentId, _managerDocumentId);
            Assert.AreEqual(commmentDto.CanModify, true);

            _employeeCommentId = commmentDto.Id;

            var editCommmentDto = EditComment(_employeeCommentId,
                _managerDocumentId,
                "Test",
                _commentContent);

            Assert.IsNotNull(editCommmentDto);
            Assert.AreEqual(editCommmentDto.Subject, "Test");
            Assert.AreEqual(editCommmentDto.CanModify, true);
        }

        [TestMethod]
        public void T03_GetDocumentsAndComments_Successfully()
        {
            var documentsDto = RetriveDocuments();

            Assert.IsNotNull(documentsDto);
            Assert.AreNotEqual(documentsDto.Count(), 0);

            var documentDto = RetriveDocument(_managerDocumentId);

            Assert.IsNotNull(documentDto);
            Assert.AreEqual(documentDto.CanModify, false);

            var commentsDto = RetriveComments(_managerDocumentId);

            Assert.IsNotNull(commentsDto);
            Assert.AreNotEqual(commentsDto.Count(), 0);

            var employeeComment = commentsDto
                .Where(c => c.Id == _employeeCommentId)
                .FirstOrDefault();

            Assert.IsNotNull(employeeComment);
            Assert.AreEqual(employeeComment.CanModify, true);

            var managerComment = commentsDto
                .Where(c => c.Id == _managerCommentId)
                .FirstOrDefault();

            Assert.IsNotNull(managerComment);
            Assert.AreEqual(managerComment.CanModify, false);
        }

        [TestMethod]
        public void T04_DeleteEmployeeComment_Successfully()
        {
            var commmentDto = RetriveComment(_employeeCommentId);

            Assert.IsNotNull(commmentDto);
            Assert.AreEqual(commmentDto.DocumentId, _managerDocumentId);
            Assert.AreEqual(commmentDto.CanModify, true);

            DeleteComment(_employeeCommentId);

            commmentDto = RetriveDeletedComment(_employeeCommentId);
            Assert.IsNull(commmentDto);

            _employeeCommentId = 0;
        }

        [TestMethod]
        public void T05_EmployeeDeleteManagerComment_Error()
        {
            DeleteCommentError(_managerCommentId);

            var commmentDto = RetriveComment(_managerCommentId);

            Assert.IsNotNull(commmentDto);
            Assert.AreEqual(commmentDto.CanModify, false);
        }

        [TestMethod]
        public void T06_EmployeeCreateDocument_Error()
        {
            AddDocumentError(_documentName, 
                _documentContent);
        }

        [TestMethod]
        public void T07_EmployeeSaveDocument_Error()
        {
            var documentDto = RetriveDocument(_managerDocumentId);

            Assert.IsNotNull(documentDto);
            Assert.AreEqual(documentDto.CanModify, false);

            EditDocumentError(_managerDocumentId, "Changed", _documentContent);

            documentDto = RetriveDocument(_managerDocumentId);

            Assert.IsNotNull(documentDto);
            Assert.AreNotEqual(documentDto.Name, "Changed");
            Assert.AreEqual(documentDto.CanModify, false);
        }

        [TestMethod]
        public void T08_EmployeeDeleteDocument_Error()
        {
            DeleteDocumentError(_managerDocumentId);

            var documentDto = RetriveDocument(_managerDocumentId);

            Assert.IsNotNull(documentDto);
            Assert.AreEqual(documentDto.CanModify, false);
        }

        [TestMethod]
        public void T09_DeleteEmployee_Successfully()
        {
            var result = DeleteUser();
            Assert.IsTrue(result);

            using (var unitOfWork = ObjectContainer.Resolve<UnitOfWork>())
            {
                var deleteUser = unitOfWork
                    .UserRepository
                    .GetAll()
                    .Where(u => u.Login == _testEmployeeUserLogin && u.Deleted)
                    .FirstOrDefault();

                Assert.IsNotNull(deleteUser);
            }
        }
    }
}