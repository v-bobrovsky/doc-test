using System;
using System.Web;
using System.Collections.Generic;

using Documents.Data;
using Documents.Services;
using Documents.Models;

namespace Documents.Core
{
    /// <summary>
    /// User context
    /// </summary>
    public class UserContext : IUserContext
    {
        private static Dictionary<int, UserDto> _users = 
            new Dictionary<int, UserDto>();

        /// <summary>
        /// Get current user id
        /// </summary>
        /// <returns></returns>
        public int GetCurrentId()
        {
            var result = 0;

            if (HttpContext.Current != null && 
                HttpContext.Current.User != null && 
                HttpContext.Current.User.Identity != null)
            {
                var userId = HttpContext
                    .Current
                    .User
                    .Identity
                    .GetUserId<string>();

                if (!Int32.TryParse(userId, out result))
                    result = 0;
            }

            return result;
        }

        /// <summary>
        /// Set current user
        /// </summary>
        /// <param name="user"></param>
        public void SetUser(UserDto user)
        {
            if (user != null)
            {
                if (!_users.ContainsKey(user.Id))
                    _users.Add(user.Id, user);
                else
                    _users[user.Id] = user;
            }
        }

        /// <summary>
        /// Remove current user
        /// </summary>
        public void ClearUser()
        {
            var userId = GetCurrentId();

            if (_users.ContainsKey(userId))
                _users.Remove(userId);
        }

        /// <summary>
        /// Get current user
        /// </summary>
        /// <returns></returns>
        public UserDto GetCurrentUser()
        {
            UserDto result = null;

            var userId = GetCurrentId();

            if (_users.ContainsKey(userId))
                result = _users[userId];

            return result;
        }
    }
}