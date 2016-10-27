using System;
using System.Linq;
using System.Collections.Generic;

using Documents.Data;
using Documents.DataAccess;
using System.Transactions;

namespace Documents.Services.Impl
{
    /// <summary>
    /// Provides users functionality.
    /// </summary>
    public class UserService: IUserService
    {
        private readonly UnitOfWork _unitOfWork;

        /// <summary>
        /// Constructor
        /// </summary>
        public UserService()
        {
            _unitOfWork = new UnitOfWork();
        }

        /// <summary>
        /// Retrieves a specific user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>
        /// <see cref="UserDto"/>s object containing the new comment.
        /// </returns>
        public UserDto GetUserById(int userId)
        {
            UserDto result = null;

            var user = _unitOfWork
                .UserRepository
                .GetByID(userId);

            if (user != null && !user.Deleted)
                result = user.ToDto();

            return result;
        }

        /// <summary>
        /// Retrieves all users
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>
        /// List of <see cref="UserDto"/>s object containing the results.
        /// </returns>
        public IEnumerable<UserDto> GetAllUsers()
        {
            List<UserDto> result = null;

            var users = _unitOfWork
                .UserRepository
                .GetMany(u => !u.Deleted)
                .ToList();

            if (users != null && users.Any())
            {
                result = new List<UserDto>();
                users.ForEach(u => result.Add(u.ToDto()));
            }

            return result;
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="user">User data</param>
        /// <returns>
        /// <see cref="UserDto"/>s object containing the new user.
        /// </returns>
        public UserDto CreateUser(UserDto user)
        {
            UserDto result = null;

            var entity = user.ToEntity();

            if (entity != null)
            {
                using (var scope = new TransactionScope())
                {
                    _unitOfWork
                        .UserRepository
                        .Insert(entity);

                    _unitOfWork.Save();
                    scope.Complete();
                }

                result = entity.ToDto();
            }

            return result;
        }

        /// <summary>
        /// Updates a specific user
        /// </summary>
        /// <param name="user">User data</param>
        /// <returns>
        /// Return True if succeeded otherwise False
        /// </returns>
        public bool UpdateUser(UserDto user)
        {
            bool result = false;

            if (user != null && user.UserId > 0)
            {
                var oldEntity = _unitOfWork
                    .UserRepository
                    .GetByID(user.UserId);

                var entity = (oldEntity != null)
                    ? user.ToEntity(oldEntity)
                    : null;

                if (entity != null)
                {
                    using (var scope = new TransactionScope())
                    {
                        _unitOfWork
                            .UserRepository
                            .Update(entity);

                        _unitOfWork.Save();
                        scope.Complete();
                    }

                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Delete a specific user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// Return True if succeeded otherwise False
        public bool DeleteUser(int userId)
        {
            bool result = false;

            if (userId > 0)
            {
                var entity = _unitOfWork
                    .UserRepository
                    .GetByID(userId);

                if (entity != null)
                {
                    using (var scope = new TransactionScope())
                    {
                        entity.Deleted = true;

                        _unitOfWork
                            .UserRepository
                            .Update(entity);

                        _unitOfWork.Save();
                        scope.Complete();
                    }

                    result = true;
                }
            }

            return result;
        }
    }
}