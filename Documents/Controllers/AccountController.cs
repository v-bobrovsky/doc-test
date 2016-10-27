using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Documents.Core;
using Documents.Models;
using Documents.Services;

namespace Documents.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// POST: api/Account/Register
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [Route("Register")]
        public IHttpActionResult Post([FromBody]RegisterViewModel login)
        {
            return Ok();
        }

        /// <summary>
        /// POST: api/Account/Login
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [Route("Login")]
        public IHttpActionResult Post([FromBody]LoginViewModel login)
        {
            return Ok();
        }

        /// <summary>
        /// POST api/Account/Logout
        /// </summary>
        /// <returns></returns>
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            //Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }
    }
}
