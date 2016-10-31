using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;

using Documents.Core;
using Documents.Models;
using Documents.Services;
using Microsoft.AspNet.Identity;
using Documents.Data;

namespace Documents.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : BaseController
    {
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

        public AccountController(ServiceUserManager serviceUserManager)
        {
            _serviceUserManager = serviceUserManager;
        }

        /// <summary>
        /// POST: api/Account/Register
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("Register")]
        public IHttpActionResult Post([FromBody]RegisterViewModel register)
        {
            return PerformAction<IdentityResult>(() =>
            {
                IdentityResult result = IdentityResult.Failed();

                var userDto = register.ToDto();
                var user = new ServiceUser(userDto);

                ServiceUserManager
                    .CreateAsync(user)
                    .ContinueWith((t) => result = t.Result);

                return result;
            });
        }

        /// <summary>
        /// POST: api/Account/Login
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [Route("Login")]
        [AllowAnonymous]
        public IHttpActionResult Post([FromBody]LoginViewModel login)
        {
            return PerformAction<UserDto>(() =>
            {
                ServiceUser user = null;

                ServiceUserManager
                    .FindAsync(login.Login, login.Password)
                    .ContinueWith((t) => user = t.Result);

                return user != null 
                    ? user.ToDto() 
                    : null;
            });
        }

        /// <summary>
        /// POST api/Account/Logout
        /// </summary>
        /// <returns></returns>
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(
                CookieAuthenticationDefaults
                .AuthenticationType);

            return Ok();
        }
    }
}
