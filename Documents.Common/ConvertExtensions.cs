using System;
using System.Collections.Generic;

using Documents.Data;
using Documents.DataAccess;

namespace Documents.Common
{
    /// <summary>
    /// Extensions methods to convert from EF entities to DTO objects and vice versa
    /// </summary>
    public static class ConvertExtensions
    {
        /// <summary>
        /// Convert entity role to dto role
        /// </summary>
        /// <param name="role">Entity role</param>
        /// <returns></returns>
        private static string ToRoleDto(Roles role)
        {
            var result = Roles.Employee.ToString();

            if (role == Roles.Manager)
                result = Roles.Manager.ToString();

            return result;
        }

        /// <summary>
        /// Convert dto role to entity role
        /// </summary>
        /// <param name="role">Dto role name</param>
        /// <returns></returns>
        private static Roles ToRoleEntity(string role)
        {
            var result = Roles.Employee;

            if (!String.IsNullOrEmpty(role) && role.Trim().ToLower() == Roles.Manager.ToString().ToLower())
                result = Roles.Manager;

            return result;
        }

        /// <summary>
        /// Convert User DTO to exist User entity
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="entity">Exist User entity</param>
        /// <returns></returns>
        public static User ToEntity(this UserDto dto, User entity)
        {
            var result = entity;

            if (result != null)
            {
                result.ModifiedTime = DateTime.Now;
                result.UserName = dto.UserName;

                if (!String.IsNullOrEmpty(dto.Password))
                    result.Password = dto.Password;
            }

            return result;
        }

        /// <summary>
        /// Convert User DTO to User entity
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public static User ToEntity(this UserDto dto)
        {
            return new User()
            {
                UserId = dto.Id,
                CreatedTime = dto.CreatedTime,
                ModifiedTime = dto.ModifiedTime,
                UserName = dto.UserName,
                Login = dto.Login,
                Password = dto.Password,
                Role = ToRoleEntity(dto.UserRole)
            };
        }

        /// <summary>
        /// Convert User entity to User DTO
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static UserDto ToDto(this User entity)
        {
            var result = !entity.Deleted ?
                new UserDto()
                {
                    Id = entity.UserId,
                    CreatedTime = entity.CreatedTime,
                    ModifiedTime = entity.ModifiedTime,
                    UserName = entity.UserName,
                    Login = entity.Login,
                    Password = entity.Password,
                    UserRole = ToRoleDto(entity.Role)
                }
                : null;

            return result;
        }

        /// <summary>
        /// Convert Comment entity to Comment DTO
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static CommentDto ToDto(this Comment entity, bool canModify)
        {
            return new CommentDto()
            {
                Id = entity.Id,
                CreatedTime = entity.CreatedTime,
                ModifiedTime = entity.ModifiedTime,
                UserName = entity.User.UserName,
                UserId = entity.User.UserId,
                CanModify = canModify,
                Subject = entity.Subject,
                Content = entity.Content,
                DocumentId = entity.DocumentId
            };
        }

        /// <summary>
        /// Convert Comment DTO to Comment entity
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public static Comment ToEntity(this CommentDto dto)
        {
            return new Comment()
            {
                Id = dto.Id,
                CreatedTime = DateTime.Now,
                ModifiedTime = DateTime.Now,
                UserId = dto.UserId,
                Subject = dto.Subject,
                Content = dto.Content,
                DocumentId = dto.DocumentId
            };
        }

        /// <summary>
        /// Convert Comment DTO to exist Comment entity
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="entity">Exist Comment entity</param>
        /// <returns></returns>
        public static Comment ToEntity(this CommentDto dto, Comment entity)
        {
            var result = entity;

            if (result != null)
            {
                result.ModifiedTime = DateTime.Now;
                result.Subject = dto.Subject;
                result.Content = dto.Content;
            }

            return result;
        }

        /// <summary>
        /// Convert Document entity to Document DTO
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public static DocumentDto ToDto(this Document entity, bool canModify)
        {
            return new DocumentDto()
            {
                Id = entity.Id,
                CreatedTime = entity.CreatedTime,
                ModifiedTime = entity.ModifiedTime,
                UserName = entity.User.UserName,
                UserId = entity.User.UserId,
                CanModify = canModify,
                Name = entity.Name,
                Content = entity.Content
            };
        }

        /// <summary>
        /// Convert Document DTO to Document entity
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public static Document ToEntity(this DocumentDto dto)
        {
            return new Document()
            {
                Id = dto.Id.Equals(Guid.Empty) ? Guid.NewGuid() : dto.Id,
                CreatedTime = DateTime.Now,
                ModifiedTime = DateTime.Now,
                UserId = dto.UserId,
                Name = dto.Name,
                Content = dto.Content
            };
        }

        /// <summary>
        /// Convert Document DTO to exist Document entity
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="entity">Exist Document entity</param>
        /// <returns></returns>
        public static Document ToEntity(this DocumentDto dto, Document entity)
        {
            var result = entity;

            if (result != null)
            {
                result.ModifiedTime = DateTime.Now;
                result.Name = dto.Name;
                result.Content = dto.Content;
            }

            return result;
        }
    }
}