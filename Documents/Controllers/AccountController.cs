using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using System.Security.Claims;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.OAuth;
using System.Security.Authentication;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Cookies;

using Documents.Core;
using Documents.Models;
using Documents.Services;
using Documents.Data;
using System.Web.Http.Description;

namespace Documents.Controllers
{
    /// <summary>
    /// Provides main functionality for users: register, login, logout.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : BaseController
    {
        private readonly IUserContext _userContext;
        private ServiceUserManager _serviceUserManager;

        [ApiExplorerSettings(IgnoreApi = true)]
        public ServiceUserManager ServiceUserManager
        {
            get
            {
                return _serviceUserManager ??
                       (_serviceUserManager = HttpContext
                       .Current
                       .GetOwinContext()
                       .GetUserManager<ServiceUserManager>());
            }
        }

        private IAuthenticationManager Authentication
        {
            get 
            { 
                return Request
                    .GetOwinContext()
                    .Authentication; 
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userContext"></param>
        public AccountController(IUserContext userContext)
        {
            _userContext = userContext;
        }
       
        /// <summary>
        /// Register new user with manager role
        /// POST: api/Account/Register/Manager
        /// </summary>
        /// <param name="data">Register data</param>
        /// <returns>
        /// <see cref="IdentityResult"/>s object containing the result of registration.
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("Register/Manager")]
        public IHttpActionResult RegisterManager([FromBody]RegisterViewModel data)
        {
            return PerformAction<IdentityResult>(() =>
            {
                return RegisterUser(data, true);
            });
        }

        /// <summary>
        /// Register new user with manager employee
        /// POST: api/Account/Register/Employee
        /// </summary>
        /// <param name="data">Register data</param>
        /// <returns>
        /// <see cref="IdentityResult"/>s object containing the result of registration.
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("Register/Employee")]
        public IHttpActionResult RegisterEmployee([FromBody]RegisterViewModel data)
        {
            return PerformAction<IdentityResult>(() =>
            {
                return RegisterUser(data, false);
            });
        }

        /// <summary>
        /// POST: api/Account/Login
        /// </summary>
        /// <param name="data">Login data</param>
        /// <returns>
        /// <see cref="IdentityResult"/>s object containing the result of registration.
        /// </returns>
        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public IHttpActionResult Login([FromBody]LoginViewModel data)
        {
            return PerformAction<IdentityResult>(() =>
            {
                ServiceUser user = null;

                var findUserTask = ServiceUserManager
                    .FindAsync(data.Login, data.Password);

                findUserTask
                    .Wait();

                user = findUserTask
                    .Result;

                Authentication
                    .SignOut();

                if (user == null)
                    throw new AuthenticationException("Invalid username or password!");

                LoginUser(user);

                return IdentityResult
                    .Success;
            });
        }

        /// <summary>
        /// Register user
        /// </summary>
        /// <param name="data">Login data</param>
        /// <param name="hasManager">Has user manager</param>
        /// <returns>
        /// <see cref="IdentityResult"/>s object containing the result of registration.
        /// </returns>
        private IdentityResult RegisterUser(RegisterViewModel data, bool hasManager)
        {
            IdentityResult result = IdentityResult
                .Failed();

            var userDto = data
                .ToDto();

            userDto.UserRole = hasManager 
                ? "Manager" : "Employee";

            var findUserTask = ServiceUserManager
                .FindByNameAsync(userDto.Login);

            findUserTask
                .Wait();

            var alreadyRegistered = (findUserTask.Result != null);

            if (alreadyRegistered)
                throw new Exception(String.Format(
                    "User {0} already exists!", userDto.Login));

            var user = new ServiceUser(userDto);

            var createUserTask = ServiceUserManager
                .CreateAsync(user);

            createUserTask
                .Wait();

            result = createUserTask
                .Result;

            if (result == IdentityResult.Success)
            {
                findUserTask = ServiceUserManager
                    .FindByNameAsync(userDto.Login);

                findUserTask
                    .Wait();

                user = findUserTask
                    .Result;

                LoginUser(user);
            }

            return result;
        }

        /// <summary>
        /// Login user
        /// </summary>
        /// <param name="user">Service user</param>
        private void LoginUser(ServiceUser user)
        {
            if (user != null)
            {
                var createIdentityTask = ServiceUserManager
                    .CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);

                createIdentityTask
                    .Wait();

                Authentication
                    .SignIn(new AuthenticationProperties
                    {
                        IsPersistent = true
                    },
                    createIdentityTask.Result);

                ((UserContext)_userContext)
                    .SetUser(user.ToDto());
            }
        }

        /// <summary>
        /// POST api/Account/Logout
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            return PerformAction<IdentityResult>(() =>
            {
                if (_userContext != null)
                    ((UserContext)_userContext)
                        .ClearUser();

                Authentication.SignOut(
                    CookieAuthenticationDefaults
                    .AuthenticationType);

                return IdentityResult
                    .Success;
            });
        }
    }
}