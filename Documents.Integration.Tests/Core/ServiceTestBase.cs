using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Transactions;

using Documents.Utils;
using Documents.DataAccess;
using System.Collections.Generic;
using Documents.Data;
using Documents.Models;
using System.Configuration;
using System.Net.Http;
using System.Net;

namespace Documents.Integration.Tests.Core
{
    public class ServiceTestBase
    {
        #region Constants

        private static readonly string _assertAccessDenied = "Access denied for /{0}.";

        private static readonly string _commentsControllerName = "comments";
        private static readonly string _documentsControllerName = "documents";
        private static readonly string _usersControllerName = "userInfo";

        private static readonly string _accountRegisterManagerControllerName = "account/register/manager";
        private static readonly string _accountRegisterEmployeeControllerName = "account/register/employee";
        private static readonly string _accountLoginControllerName = "account/login";
        private static readonly string _accountLogoutControllerName = "account/logout";
       
        #endregion

        #region Members

        protected readonly ILogger _logger;
        protected readonly WebApiHttpClient _webApiClient;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public ServiceTestBase()
        {
            _logger = ObjectContainer.Resolve<SimpleLogger>();

            var webApiAddress = ConfigurationManager
                .AppSettings["WebApiAddress"];

            _webApiClient = new WebApiHttpClient(_logger, webApiAddress);
        }

        /// <summary>
        /// Validate access permissions
        /// </summary>
        /// <param name="response"></param>
        private void ValidatePermissions(WebApiResponse response)
        {
            Assert.AreNotEqual(response.Code,
                HttpStatusCode.Unauthorized,
                _assertAccessDenied);

            Assert.AreNotEqual(response.Code,
                HttpStatusCode.Forbidden,
                _assertAccessDenied);
        }

        /// <summary>
        /// Retrive data by web api
        /// </summary>
        /// <typeparam name="T">Request data type</typeparam>
        /// <param name="controller">Controller name</param>
        /// <param name="data">Request data</param>
        /// <returns>
        /// <see cref="WebApiResponse"/>s object containing the web api response.
        /// </returns>
        protected WebApiResponse RetriveDataByApi<T>(string controller, T data)
            where T : class
        {
            var request = new WebApiRequest<T>(controller, 
                HttpMethod.Get, data);

            var response = _webApiClient
                .Send<T>(request);

            ValidatePermissions(response);

            return response;
        }

        /// <summary>
        /// Create data by web api
        /// </summary>
        /// <typeparam name="T">Request data type</typeparam>
        /// <param name="controller">Controller name</param>
        /// <param name="data">Request data</param>
        /// <returns>
        /// <see cref="WebApiResponse"/>s object containing the web api response.
        /// </returns>
        protected WebApiResponse CreateDataByApi<T>(string controller, T data)
            where T : class
        {
            var request = new WebApiRequest<T>(controller, 
                HttpMethod.Post, data);

            var response = _webApiClient
                .Send<T>(request);

            ValidatePermissions(response);

            return response;
        }

        /// <summary>
        /// Update data by web api
        /// </summary>
        /// <typeparam name="T">Request data type</typeparam>
        /// <param name="controller">Controller name</param>
        /// <param name="data">Request data</param>
        /// <returns>
        /// <see cref="WebApiResponse"/>s object containing the web api response.
        /// </returns>
        protected WebApiResponse UpdateDataByApi<T>(string controller, T data)
            where T : class
        {
            var request = new WebApiRequest<T>(controller,
                HttpMethod.Put, data);

            var response = _webApiClient
                .Send<T>(request);

            ValidatePermissions(response);

            return response;
        }

        /// <summary>
        /// Remove data by web api
        /// </summary>
        /// <param name="controller">Controller name</param>
        /// <param name="data">Request data</param>
        /// <returns>
        /// <see cref="WebApiResponse"/>s object containing the web api response.
        /// </returns>
        protected WebApiResponse RemoveDataByApi(string controller, string data)
        {
            var request = new WebApiRequest<string>(controller,
                HttpMethod.Delete, data);

            var response = _webApiClient
                .Send<string>(request);

            ValidatePermissions(response);

            return response;
        }

        /// <summary>
        /// Login user
        /// </summary>
        /// <param name="login">User login</param>
        /// <param name="passsword">User password</param>
        /// <returns></returns>
        protected void LoginUser(string login, string passsword)
        {
            var response = CreateDataByApi<LoginViewModel>(
                _accountLoginControllerName,
                new LoginViewModel()
                {
                    Login = login,
                    Password = passsword
                });

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Result);
        }

        /// <summary>
        /// Logout user
        /// </summary>
        protected void LogoutUser()
        {
            var response = CreateDataByApi<string>(
                _accountLogoutControllerName, null);

            _webApiClient
                .ClearSession();
        }
      
        /// <summary>
        /// Register new user by web api
        /// </summary>
        /// <param name="name">User name</param>
        /// <param name="login">User login</param>
        /// <param name="passsword">User password</param>
        /// <param name="hasManagerRole">If true user is Manager otherwise Employee</param>
        protected void RegisterUser(string name, string login, string passsword, bool hasManagerRole)
        {
            var response = CreateDataByApi<RegisterViewModel>(
                hasManagerRole
                ? _accountRegisterManagerControllerName 
                : _accountRegisterEmployeeControllerName,
                new RegisterViewModel()
                {
                    UserName = name,
                    Login = login,
                    Password = passsword
                });

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Result);
        }

        /// <summary>
        /// Retrive exists comment by web api
        /// </summary>
        /// <param name="id">Comment id</param>
        /// <returns>
        /// <see cref="CommentDto"/>s object containing the new comment.
        /// </returns>
        protected CommentDto RetriveComment(int id)
        {
            var response = RetriveDataByApi<string>(
                    _commentsControllerName,
                    id.ToString());

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Result);

            return response
                .To<CommentDto>();
        }

        /// <summary>
        /// Retrive deleted comment by web api
        /// </summary>
        /// <param name="id">Comment id</param>
        /// <returns>
        /// <see cref="CommentDto"/>s object containing the new comment.
        /// </returns>
        protected CommentDto RetriveDeletedComment(int id)
        {
            var response = RetriveDataByApi<string>(
                    _commentsControllerName,
                    id.ToString());

            Assert.IsNotNull(response);
            Assert.IsFalse(response.Result);

            return response
                .To<CommentDto>();
        }

        /// <summary>
        /// Retrive current user
        /// </summary>
        /// <returns>
        /// <see cref="UserDto"/>s object containing the logged user dto.
        /// </returns>
        protected UserDto RetriveUser()
        {
            var response = RetriveDataByApi<string>(
                    _usersControllerName,
                    string.Empty);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Result);

            return response
                .To<UserDto>();
        }

        /// <summary>
        /// Edit exists user by web api
        /// </summary>
        /// <param name="name">User name</param>
        /// <param name="login">User login</param>
        /// <param name="passsword">User password. If password empty the password will not change.</param>
        /// <returns>
        /// <see cref="UserDto"/>s object containing the logged user dto.
        /// </returns>
        protected UserDto EditUser(string name, string login, string passsword)
        {
            var response = UpdateDataByApi<UserProfileViewModel>(
                _usersControllerName,
                new UserProfileViewModel()
                {
                    UserName = name,
                    Login = login,
                    Password = passsword
                });

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Result);

            return response
                .To<UserDto>();
        }

        /// <summary>
        /// Delete exists user by web api
        /// </summary>
        /// <returns>
        /// True is sucessfully otherwise False.
        /// </returns>
        protected bool DeleteUser()
        {
            var response = RemoveDataByApi(
                _usersControllerName, string.Empty);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Result);

            return response
                .To<bool>();
        }

        /// <summary>
        /// Retrive comments by web api
        /// </summary>
        /// <param name="documentId">Document Id</param>
        /// <returns>
        /// <see cref="CommentDto"/>s object containing the new comment.
        /// </returns>
        protected IEnumerable<CommentDto> RetriveComments(Guid documentId)
        {
            var response = RetriveDataByApi<string>(
                String.Format("{0}/?documentId=",
                    _commentsControllerName),
                documentId.ToString());

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Result);

            return response
                .To<IEnumerable<CommentDto>>();
        }

        /// <summary>
        /// Add new comment by web api
        /// </summary>
        /// <param name="documentId">Document Id</param>
        /// <param name="subject">Comnment subject</param>
        /// <param name="text">Comnment text</param>
        /// <returns>
        /// <see cref="CommentDto"/>s object containing the new comment.
        /// </returns>
        protected CommentDto AddComment(Guid documentId, string subject, string text)
        {
            var response = CreateDataByApi<CommentViewModel>(
                _commentsControllerName,
                new CommentViewModel()
                {
                    DocumentId = documentId,
                    Subject = subject,
                    Text = text
                });

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Result);

            return response
                .To<CommentDto>();
        }

        /// <summary>
        /// Retrive deleted exists document by web api
        /// </summary>
        /// <param name="id">Document Id</param>
        /// <returns>
        /// <see cref="DocumentDto"/>s object containing the new comment.
        /// </returns>
        protected DocumentDto RetriveDeletedDocument(Guid id)
        {
            var response = RetriveDataByApi<string>(
                    _documentsControllerName,
                    id.ToString());

            Assert.IsNotNull(response);
            Assert.IsFalse(response.Result);

            return response
                .To<DocumentDto>();
        }

        /// <summary>
        /// Retrive exists document by web api
        /// </summary>
        /// <param name="id">Document Id</param>
        /// <returns>
        /// <see cref="DocumentDto"/>s object containing the new comment.
        /// </returns>
        protected DocumentDto RetriveDocument(Guid id)
        {
            var response = RetriveDataByApi<string>(
                    _documentsControllerName,
                    id.ToString());

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Result);

            return response
                .To<DocumentDto>();
        }

        /// <summary>
        /// Retrive documents by web api
        /// </summary>
        /// <param name="documentId">Document Id</param>
        /// <returns>
        /// <see cref="CommentDto"/>s object containing the new comment.
        /// </returns>
        protected IEnumerable<DocumentDto> RetriveDocuments()
        {
            var response = RetriveDataByApi<string>(
                _documentsControllerName, 
                string.Empty);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Result);

            return response
                .To<IEnumerable<DocumentDto>>();
        }

        /// <summary>
        /// Add new document by web api
        /// </summary>
        /// <param name="name">Document name</param>
        /// <param name="content">Document text</param>
        /// <returns>
        /// <see cref="DocumentDto"/>s object containing the new document.
        /// </returns>
        protected DocumentDto AddDocument(string name, string content)
        {
            var response = CreateDataByApi<DocumentViewModel>(
                _documentsControllerName,
                new DocumentViewModel()
                {
                    Name = name,
                    Content = content
                });

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Result);

            return response
                .To<DocumentDto>();
        }

        /// <summary>
        /// Add new document error by web api
        /// </summary>
        /// <param name="name">Document name</param>
        /// <param name="content">Document text</param>
        protected void AddDocumentError(string name, string content)
        {
            var response = CreateDataByApi<DocumentViewModel>(
                _documentsControllerName,
                new DocumentViewModel()
                {
                    Name = name,
                    Content = content
                });

            Assert.IsNotNull(response);
            Assert.IsFalse(response.Result);
        }

        /// <summary>
        /// Edit exists comment by web api
        /// </summary>
        /// <param name="id">Comment id</param>
        /// <param name="documentId">Document Id</param>
        /// <param name="subject">Comnment subject</param>
        /// <param name="text">Comnment text</param>
        /// <returns>
        /// <see cref="CommentDto"/>s object containing the new comment.
        /// </returns>
        protected CommentDto EditComment(int id, Guid documentId, string subject, string text)
        {
            var response = UpdateDataByApi<CommentViewModel>(
                String.Format("{0}/{1}", 
                    _commentsControllerName,
                    id),
                new CommentViewModel()
                {
                    DocumentId = documentId,
                    Subject = subject,
                    Text = text
                });

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Result);

            return response
                .To<CommentDto>();
        }

        /// <summary>
        /// Edit exist document by web api
        /// </summary>
        /// <param name="documentId">Document Id</param>
        /// <param name="name">Document name</param>
        /// <param name="content">Document text</param>
        /// <returns>
        /// <see cref="DocumentDto"/>s object containing the new document.
        /// </returns>
        protected DocumentDto EditDocument(Guid documentId, string name, string content)
        {
            var response = CreateDataByApi<DocumentViewModel>(
                _documentsControllerName,
                new DocumentViewModel()
                {
                    Name = name,
                    Content = content
                });

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Result);

            return response
                .To<DocumentDto>();
        }

        /// <summary>
        /// Edit exist document error by web api
        /// </summary>
        /// <param name="documentId">Document Id</param>
        /// <param name="name">Document name</param>
        /// <param name="content">Document text</param>
        /// <returns>
        /// <see cref="DocumentDto"/>s object containing the new document.
        /// </returns>
        protected void EditDocumentError(Guid documentId, string name, string content)
        {
            var response = CreateDataByApi<DocumentViewModel>(
                _documentsControllerName,
                new DocumentViewModel()
                {
                    Name = name,
                    Content = content
                });

            Assert.IsNotNull(response);
            Assert.IsFalse(response.Result);
        }

        /// <summary>
        /// Delete exists comment by web api
        /// </summary>
        /// <param name="id">Comment id</param>
        /// <returns>
        /// True is successfully otherwise False
        /// </returns>
        protected bool DeleteComment(int id)
        {
            var response = RemoveDataByApi(
                _commentsControllerName,
                id.ToString());

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Result);

            return response
                .To<bool>();
        }

        /// <summary>
        /// Delete comment error by web api
        /// </summary>
        /// <param name="id">Comment id</param>
        protected void DeleteCommentError(int id)
        {
            var response = RemoveDataByApi(
                _commentsControllerName,
                id.ToString());

            Assert.IsNotNull(response);
            Assert.IsFalse(response.Result);
        }

        /// <summary>
        /// Delete exists document by web api
        /// </summary>
        /// <param name="documentId">Document Id</param>
        protected bool DeleteDocument(Guid documentId)
        {
            var response = RemoveDataByApi(
                _documentsControllerName,
                documentId.ToString());

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Result);

            return response
                .To<bool>();
        }

        /// <summary>
        /// Delete exists document error by web api
        /// </summary>
        /// <param name="documentId">Document Id</param>
        protected void DeleteDocumentError(Guid documentId)
        {
            var response = RemoveDataByApi(
                _documentsControllerName,
                documentId.ToString());

            Assert.IsNotNull(response);
            Assert.IsFalse(response.Result);
        }
    }
}