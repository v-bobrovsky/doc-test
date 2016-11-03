using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity.Owin;

using System.Security.Claims;
using System.Security.Authentication;

using Documents.Core;
using Documents.Models;
using Documents.Services;
using Documents.Data;

namespace Documents.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : BaseController
    {
        private readonly IUserContext _userContext;
        private ServiceUserManager _serviceUserManager;

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

        public AccountController(IUserContext userContext)
        {
            _userContext = userContext;
        }

        /// <summary>
        /// POST: api/Account/Register/Manager
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("Register/Employee")]
        public IHttpActionResult Register()
        {
            return RegisterEmployee(new RegisterViewModel()
                {
                    Login = "asa@asa.xxx",
                    Password = "z1213232!",
                    UserName = "23232"
                });
        }

        /// <summary>
        /// POST: api/Account/Register/Manager
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("Register/Manager")]
        public IHttpActionResult RegisterManager([FromBody]RegisterViewModel register)
        {
            return PerformAction<IdentityResult>(() =>
            {
                return RegisterUser(register, true);
            });
        }

        /// <summary>
        /// POST: api/Account/Register/Employee
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("Register/Employee")]
        public IHttpActionResult RegisterEmployee([FromBody]RegisterViewModel register)
        {
            return PerformAction<IdentityResult>(() =>
            {
                return RegisterUser(register, false);
            });
        }

        /// <summary>
        /// POST: api/Account/Login
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public IHttpActionResult LoginUser([FromBody]LoginViewModel login)
        {
            return PerformAction<IdentityResult>(() =>
            {
                ServiceUser user = null;

                ServiceUserManager
                    .FindAsync(login.Login, login.Password)
                    .ContinueWith((t) => user = t.Result);

                Authentication
                    .SignOut();

                if (user == null)
                    throw new AuthenticationException("Invalid username or password!");

                LogInUserSync(user);

                return IdentityResult
                    .Success;
            });
        }

        /// <summary>
        /// Register user
        /// </summary>
        /// <param name="register"></param>
        /// <param name="hasManager"></param>
        /// <returns></returns>
        public IdentityResult RegisterUser(RegisterViewModel register, bool hasManager)
        {
            IdentityResult result = IdentityResult
                .Failed();

            var userDto = register
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

            result = createUserTask.Result;

            if (result == IdentityResult.Success)
            {
                findUserTask = ServiceUserManager
                    .FindByNameAsync(userDto.Login);

                findUserTask
                    .Wait();

                user = findUserTask
                    .Result;

                LogInUserSync(user);
            }

            return result;
        }


        /// <summary>
        /// Login user sync
        /// </summary>
        /// <param name="user">Service user</param>
        private void LogInUserSync(ServiceUser user)
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
        [HttpGet]
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