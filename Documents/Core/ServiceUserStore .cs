﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

using Documents.Models;
using Documents.Services;
using Documents.Utils;

namespace Documents.Core
{
    /// <summary>
    /// Class that implemented basic user management apis
    /// </summary>
    public class ServiceUserStore : IUserStore<ServiceUser>
    {
        #region Members

        protected readonly ILogger _logger;
        private readonly IUserService _userService;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public ServiceUserStore(ILogger logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        /// <summary>
        /// Performs async the specified action
        /// </summary>
        /// <param name="action">Action to perform</param>
        private Task PerformAsyncAction(Action action)
        {
            var task = Task
                .Factory
                .StartNew(() => 
                    {
                        try
                        {
                            _logger.LogInfo(String.Format("Performing method on ServiceUserStore: {0}", 
                                action.Method.Name));

                            action();
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e);
                            throw e;
                        }
                    });

            return task;
        }

        /// <summary>
        /// Performs async the specified function and returns Task<ServiceUser>
        /// </summary>
        /// <param name="action">action to perform</param>
        /// <returns>Function result</returns>
        protected Task<ServiceUser> PerformAsyncFunction(Func<ServiceUser> action)
        {
            var task = Task<ServiceUser>
                .Factory
                .StartNew(() => 
                    {
                        try
                        {
                            _logger.LogInfo(String.Format("Performing method on ServiceUserStore: {0}", 
                                action.Method.Name));

                            return action();
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e);
                            throw e;
                        }
                    });

            return task;
        }

        public Task CreateAsync(ServiceUser user)
        {
            return PerformAsyncAction(() => 
            {
                var userDto = user.ToDto();

                userDto = _userService.Create(userDto);

                if (userDto == null)
                    throw new Exception(String.Format(
                        "Can't create new user with login: {0}",
                        user.UserName));
            });
        }

        public Task DeleteAsync(ServiceUser user)
        {
            return PerformAsyncAction(() =>
            {
                var userDto = user.ToDto();

                if (!_userService.Delete(userDto.Id))
                    throw new Exception(String.Format(
                        "Can't delete user with Id: {0}, login: {1}",
                        user.Id, user.UserName));
            });
        }

        public Task<ServiceUser> FindByIdAsync(string userId)
        {
            return PerformAsyncFunction(() =>
            {
                ServiceUser user = null;

                if (!String.IsNullOrEmpty(userId))
                {
                    int id;

                    if (!Int32.TryParse(userId, out id))
                        id = 0;

                    var userDto = _userService.Get(id);

                    if (userDto != null)
                        user = new ServiceUser(userDto);
                }

                return user;
            });
        }

        public Task<ServiceUser> FindByNameAsync(string userName)
        {
            return PerformAsyncFunction(() =>
            {
                ServiceUser user = null;

                if (!String.IsNullOrEmpty(userName))
                {
                    var usersDto = _userService.GetAll(userName);

                    if (usersDto != null && !usersDto.Any())
                        user = new ServiceUser(
                            usersDto
                            .FirstOrDefault());
                }

                return user;
            });

        }

        public Task UpdateAsync(ServiceUser user)
        {
            return PerformAsyncAction(() =>
            {
                var userDto = user.ToDto();

                userDto = _userService.Update(userDto);

                if (userDto == null)
                    throw new Exception(String.Format(
                        "Can't delete user with Id: {0}, login: {1}",
                        user.Id, user.UserName));
            });
        }

        public void Dispose()
        {
        }
    }
}