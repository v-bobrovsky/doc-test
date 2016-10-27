﻿using System.Collections.Generic;
using System.Web.Http;

using Documents.Core;
using Documents.Data;
using Documents.Filters;
using Documents.Models;
using Documents.Services;

namespace Documents.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// GET: api/Users
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult Get()
        {
            return PerformAction<IEnumerable<UserDto>>(() =>
            {
                return _userService.GetAllUsers();
            });
        }

        /// <summary>
        /// GET: api/Users/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserPermissions(ContentOwner = "Own")]
        public IHttpActionResult Get(int id)
        {
            return PerformAction<UserDto>(() =>
            {
                return _userService.GetUserById(id);
            });
        }

        /// <summary>
        /// PUT: api/Users/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [UserPermissions(ContentOwner = "Own")]
        public IHttpActionResult Put(int id, [FromBody]UserViewModel value)
        {
            return PerformAction<bool>(() =>
            {
                var user = _userService.GetUserById(id);
                user = value.ToDto(user);
                return _userService.UpdateUser(user);
            });
        }

        /// <summary>
        /// DELETE: api/Users/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserPermissions(ContentOwner = "Own")]
        public IHttpActionResult Delete(int id)
        {
            return PerformAction<bool>(() =>
            {
                return _userService.DeleteUser(id);
            });
        }
    }
}