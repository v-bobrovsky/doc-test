using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Documents.Integration.Tests.Core;
using System.Threading;
using Documents.DataAccess;
using System.Transactions;

namespace Documents.Integration.Tests
{
    [TestClass]
    public class ManagerTest
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

        private static int _employeeUserId = 0;
        private static int _managerCommentId = 0;
        private static int _employeeCommentId = 0;
        private static Guid _managerDocumentId = Guid.Empty;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public ManagerTest()
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
                //Add employee user
                var employeeUser = new User()
                {
                    CreatedTime = DateTime.Now,
                    ModifiedTime = DateTime.Now,
                    Login = _testEmployeeUserLogin,
                    UserName = _testEmployeeUserName,
                    Password = _testEmployeeUserPassword,
                    Role = Roles.Employee
                };

                using (var scope = new TransactionScope())
                {
                    unitOfWork
                        .UserRepository
                        .Insert(employeeUser);

                    unitOfWork.Save();
                    scope.Complete();
                }

                _employeeUserId = employeeUser.UserId;
            }
        }

        private void AddEmployeeComment()
        {
            using (var unitOfWork = ObjectContainer.Resolve<UnitOfWork>())
            {
                //Add new comment from employee
                var comment = new Comment()
                {
                    CreatedTime = DateTime.Now,
                    ModifiedTime = DateTime.Now,
                    DocumentId = _managerDocumentId,
                    UserId = _employeeUserId,
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

                _employeeCommentId = comment.Id;
            }
        }

        [TestMethod]
        public void T10_RegisterManager_Successfully()
        {
            SetupTestData();

            LogoutUser();

            Thread
                .Sleep(1000);

            RegisterUser(_testManagerUserName,
                _testManagerUserLogin,
                _testManagerUserPassword,
                true);

            var userDto = RetriveUser();

            Assert.IsNotNull(userDto);
            Assert.AreEqual(userDto.Login, _testManagerUserLogin);

            LogoutUser();

            Thread
                .Sleep(1000);
        }

        [TestMethod]
        public void T12_SaveManagerDocument_Successfully()
        {
            LoginUser(_testManagerUserLogin,
                _testManagerUserPassword);

            var documentDto = AddDocument(_documentName,
                _documentContent);

            Assert.IsNotNull(documentDto);
            Assert.AreEqual(documentDto.Name, _documentName);
            Assert.AreEqual(documentDto.Content, _documentContent);
            Assert.AreEqual(documentDto.CanModify, true);

            _managerDocumentId = documentDto.Id;

            var editDocumentDto = EditDocument(_managerDocumentId,
                "Test",
                _documentContent);

            Assert.IsNotNull(editDocumentDto);
            Assert.AreEqual(editDocumentDto.Name, "Test");
            Assert.AreEqual(editDocumentDto.CanModify, true);
        }

        [TestMethod]
        public void T13_DeleteManagerDocumentWithComments_Successfully()
        {
            AddEmployeeComment();

            var commmentDto = AddComment(_managerDocumentId,
                _commentSubj,
                _commentContent);

            Assert.IsNotNull(commmentDto);
            Assert.AreEqual(commmentDto.DocumentId, _managerDocumentId);
            Assert.AreEqual(commmentDto.CanModify, true);

            _managerCommentId = commmentDto.Id;

            DeleteDocument(_managerDocumentId);
            
            commmentDto = RetriveDeletedComment(_employeeCommentId);
            Assert.IsNull(commmentDto);

            commmentDto = RetriveDeletedComment(_managerCommentId);
            Assert.IsNull(commmentDto);

            var documentDto = RetriveDeletedDocument(_managerDocumentId);
            Assert.IsNull(documentDto);
        }

        [TestMethod]
        public void T14_CleanupIntegration()
        {
            DatabaseHelper
                .CleanupForUser(_testManagerUserLogin);

            DatabaseHelper
                .CleanupForUser(_testEmployeeUserLogin);
        }
    }
}
