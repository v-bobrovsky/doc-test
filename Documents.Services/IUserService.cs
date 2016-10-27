using System;
using System.Collections.Generic;

using Documents.Data;

namespace Documents.Services
{
    /// <summary>
    /// Provides users functionality.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Retrieves a specific user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>
        /// <see cref="UserDto"/>s object containing the new comment.
        /// </returns>
        UserDto GetUserById(int userId);

        /// <summary>
        /// Retrieves all users.
        /// </summary>
        /// <returns>
        /// List of <see cref="UserDto"/>s object containing the results.
        /// </returns>
        IEnumerable<UserDto> GetAllUsers();

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="user">User data</param>
        /// <returns>
        /// <see cref="UserDto"/>s object containing the new user.
        /// </returns>
        UserDto CreateUser(UserDto user);

        /// <summary>
        /// Updates a specific user
        /// </summary>
        /// <param name="user">User data</param>
        /// <returns>
        /// Return True if succeeded otherwise False
        /// </returns>
        bool UpdateUser(UserDto user);

        /// <summary>
        /// Delete a specific user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// Return True if succeeded otherwise False
        bool DeleteUser(int userId);
    }
}