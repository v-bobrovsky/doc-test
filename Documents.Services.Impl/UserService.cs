using System;
using System.Linq;
using System.Transactions;
using System.Collections.Generic;

using Documents.Data;
using Documents.Common;
using Documents.DataAccess;
using Documents.Utils;

namespace Documents.Services.Impl
{
    /// <summary>
    /// Provides users functionality.
    /// </summary>
    public class UserService : RepositoryService<UserDto, int>, IUserService
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public UserService(IUserContext userCtx)
            : base(userCtx)
        {
        }

        /// <summary>
        /// Validate user identity
        /// </summary>
        /// <param name="id">User identity</param>
        /// <returns></returns>
        protected override bool ValidateEntityKey(int id)
        {
            return (id > 0);
        }

        /// <summary>
        /// Validate new user entity
        /// </summary>
        /// <param name="entityDto">User entity</param>
        /// <returns></returns>
        protected override bool ValidateNewEntity(UserDto entityDto)
        {
            return entityDto != null
                && !String.IsNullOrEmpty(entityDto.Login)
                && !String.IsNullOrEmpty(entityDto.Password)
                && !String.IsNullOrEmpty(entityDto.UserName)
                /*&& !String.IsNullOrEmpty(entityDto.UserRole)*/;
        }

        /// <summary>
        /// Validate exist user entity
        /// </summary>
        /// <param name="entityDto">User entity</param>
        /// <returns></returns>
        protected override bool ValidateExistEntity(UserDto entityDto)
        {
            return entityDto != null
                && entityDto.Id > 0
                && !String.IsNullOrEmpty(entityDto.Login)
                && !String.IsNullOrEmpty(entityDto.Password)
                && !String.IsNullOrEmpty(entityDto.UserName)
                /*&& !String.IsNullOrEmpty(entityDto.UserRole)*/;
        }
 
        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="user">User data</param>
        /// <returns>
        /// <see cref="UserDto"/>s object containing the new user.
        /// </returns>
        protected override UserDto OnCreate(UserDto entityDto)
        {
            UserDto result = null;

            var entity = entityDto.ToEntity();

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
        /// Retrieves a specific user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>
        /// <see cref="UserDto"/>s object containing the new comment.
        /// </returns>
        protected override UserDto OnGet(int id)
        {
            UserDto result = null;

            var user = _unitOfWork
                .UserRepository
                .GetByID(id);

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
        protected override IEnumerable<UserDto> OnGetAll(params object[] args)
        {
            return GetAllUsers();
        }

        /// <summary>
        /// Retrieves all users
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>
        /// List of <see cref="UserDto"/>s object containing the results.
        /// </returns>
        private IEnumerable<UserDto> GetAllUsers()
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
        /// Updates a specific user
        /// </summary>
        /// <param name="user">User data</param>
        /// <returns>
        /// Return True if succeeded otherwise False
        /// </returns>
        protected override UserDto OnUpdate(UserDto entityDto)
        {
            UserDto result = null;

            var oldEntity = _unitOfWork
                .UserRepository
                .GetByID(entityDto.Id);

            var entity = (oldEntity != null)
                ? entityDto.ToEntity(oldEntity)
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

                result = entity.ToDto();
            }

            return result;
        }

        /// <summary>
        /// Delete a specific user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// Return True if succeeded otherwise False
        protected override bool OnDelete(int id)
        {
            bool result = false;

            var entity = _unitOfWork
                .UserRepository
                .GetByID(id);

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

            return result;
        }
    }
}