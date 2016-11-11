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
using Documents.Data;
using Documents.Filters;
using Documents.Models;
using Documents.Services;

namespace Documents.Controllers
{
    /// <summary>
    /// Provides base functionality for logged user information.
    /// </summary>
    [Authorize]
    public class UserInfoController : BaseController
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        public UserInfoController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Retrives a logged user information.
        /// GET: api/UserInfo
        /// </summary>
        /// <returns>
        /// <see cref="UserDto"/>s object containing the user dto entity.
        /// </returns>
        [HttpGet]
        public IHttpActionResult Retrive()
        {
            return PerformAction<UserDto>(() =>
            {
                var ctx = GetPermissionsContext();

                var userDto = _userService
                    .Get(ctx, ctx.CurrentUserId);

                if (userDto != null)
                {
                    userDto.CanModify = true;
                    userDto.Password = string.Empty;
                }

                return userDto;
            });
        }

        /// <summary>
        /// Updates a logged user information. 
        /// PUT: api/UserInfo
        /// </summary>
        /// <param name="data">User profile information. 
        /// Password will be changed when password field is not empty.</param>
        /// <returns>
        /// <see cref="UserDto"/>s object containing the user dto entity.
        /// </returns>
        [HttpPut]
        public IHttpActionResult Update([FromBody]UserProfileViewModel data)
        {
            return PerformAction<UserDto>(() =>
            {
                UserDto result = null;

                var ctx = GetPermissionsContext();

                var userDto = _userService
                    .Get(ctx, ctx.CurrentUserId);

                if (userDto != null)
                {
                    userDto = data
                        .ToDto(userDto);

                    result = _userService
                        .Update(userDto);
                }

                return result;
            });
        }

        /// <summary>
        /// Delete a logged user information. 
        /// DELETE: api/UserInfo
        /// </summary>
        /// <returns>
        /// True if successfully otherwise False.
        /// </returns>
        [HttpDelete]
        public IHttpActionResult Delete()
        {
            return PerformAction<bool>(() =>
            {
                var userId = GetCurrentUserId();

                var result = _userService
                    .Delete(userId);

                if (result)
                {
                    Authentication.SignOut(
                        CookieAuthenticationDefaults
                        .AuthenticationType);
                }

                return result;
            });
        }
    }
}