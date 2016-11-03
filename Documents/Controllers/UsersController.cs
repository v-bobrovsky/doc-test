using System.Collections.Generic;
using System.Web.Http;

using Documents.Core;
using Documents.Data;
using Documents.Filters;
using Documents.Models;
using Documents.Services;

namespace Documents.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUserContext _userContext;
        private readonly IUserService _userService;

        public UsersController(IUserContext userContext, IUserService userService)
        {
            _userContext = userContext;
            _userService = userService;
        }

        /// <summary>
        /// GET: api/Users
        /// </summary>
        /// <returns></returns>
        [UserPermissions(ContentOwner = "Own")]
        public IHttpActionResult Get()
        {
            return PerformAction<UserDto>(() =>
            {
                var userId = _userContext
                    .GetCurrentId();

                var userDto = _userService
                    .Get(userId);

                userDto.Password = string.Empty;

                return userDto;
            });
        }

        /// <summary>
        /// PUT: api/Users/5
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [UserPermissions(ContentOwner = "Own")]
        public IHttpActionResult Put([FromBody]UserProfileViewModel value)
        {
            return PerformAction<UserDto>(() =>
            {
                var userDto = value
                    .ToDto();

                userDto.Id = _userContext
                    .GetCurrentId();

                return _userService
                    .Update(userDto);
            });
        }

        /// <summary>
        /// DELETE: api/Users
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserPermissions(ContentOwner = "Own")]
        public IHttpActionResult Delete()
        {
            return PerformAction<bool>(() =>
            {
                var userId = _userContext
                    .GetCurrentId();

                return _userService
                    .Delete(userId);
            });
        }
    }
}