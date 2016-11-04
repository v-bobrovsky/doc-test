using System;

using Documents.Data;
using Documents.Models;
using System.Security.Principal;
using System.Security.Claims;
using System.Globalization;

namespace Documents.Core
{
    /// <summary>
    /// Extensions methods to convert web api model to DTO objects
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Get user id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static T GetUserId<T>(this IIdentity identity) 
            where T : IConvertible
        {
            var result = default(T);

            if (identity == null)
                throw new ArgumentNullException("identity");

            var claims = identity 
                as ClaimsIdentity;

            if (claims != null)
            {
                var id = claims
                    .FindFirst(ClaimTypes.NameIdentifier);

                if (id != null)
                    result = (T)Convert.ChangeType(
                        id.Value, 
                        typeof(T), 
                        CultureInfo.InvariantCulture);
            }

            return result;
        }

        /// <summary>
        /// Get user role
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static string GetUserRole(this IIdentity identity)
        {
            var role = string.Empty;

            if (identity == null)
                throw new ArgumentNullException("identity");

            var claims = identity 
                as ClaimsIdentity;

            if (claims != null)
            {
                var id = claims
                    .FindFirst(ClaimsIdentity.DefaultRoleClaimType);

                if (id != null)
                    role = id.Value;
            }

            return role;
        }

        /// <summary>
        /// Convert to UserDto
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static UserDto ToDto(this RegisterViewModel model)
        {
            return new UserDto()
            {
                CreatedTime = DateTime.Now,
                ModifiedTime = DateTime.Now,
                Login = model.Login,
                UserName = model.UserName,
                Password = model.Password
            };
        }
        
        /// <summary>
        /// Convert to UserDto
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static UserDto ToDto(this UserProfileViewModel model)
        {
            return new UserDto()
            {
                CreatedTime = DateTime.Now,
                ModifiedTime = DateTime.Now,
                Login = model.Login,
                UserName = model.UserName,
                Password = model.Password
            };
        }

        /// <summary>
        /// Convert to UserDto
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userDto"></param>
        /// <returns></returns>
        public static UserDto ToDto(this UserProfileViewModel model, UserDto userDto)
        {
            return new UserDto()
            {
                Id = userDto.Id,
                CreatedTime = userDto.CreatedTime,
                ModifiedTime = DateTime.Now,
                Login = userDto.Login,
                UserName = model.UserName,
                Password = !String.IsNullOrEmpty(model.Password) ? model.Password : userDto.Password,
                UserRole = userDto.UserRole
            };
        }

        /// <summary>
        /// Convert to CommentDto
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static CommentDto ToDto(this CommentViewModel model)
        {
            return new CommentDto()
            {
                CreatedTime = DateTime.Now,
                ModifiedTime = DateTime.Now,
                Subject = model.Subject,
                Content = model.Text,
                DocumentId = model.DocumentId
            };
        }

        /// <summary>
        /// Convert to CommentDto
        /// </summary>
        /// <param name="model"></param>
        /// <param name="commentDto"></param>
        /// <returns></returns>
        public static CommentDto ToDto(this CommentViewModel model, CommentDto commentDto)
        {
            return new CommentDto()
            {
                Id = commentDto.Id,
                CreatedTime = commentDto.CreatedTime,
                ModifiedTime = DateTime.Now,
                Subject = model.Subject,
                Content = model.Text,
                DocumentId = commentDto.DocumentId,
                UserId = commentDto.UserId
            };
        }

        /// <summary>
        /// Convert to DocumentDto
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static DocumentDto ToDto(this DocumentViewModel model)
        {
            return new DocumentDto()
            {
                CreatedTime = DateTime.Now,
                ModifiedTime = DateTime.Now,
                Name = model.Name,
                Content = model.Content
            };
        }

        /// <summary>
        /// Convert to DocumentDto
        /// </summary>
        /// <param name="model"></param>
        /// <param name="documentDto"></param>
        /// <returns></returns>
        public static DocumentDto ToDto(this DocumentViewModel model, DocumentDto documentDto)
        {
            return new DocumentDto()
            {
                Id = documentDto.Id,
                CreatedTime = documentDto.CreatedTime,
                ModifiedTime = DateTime.Now,
                Name = model.Name,
                Content = model.Content,
                UserId = documentDto.UserId
            };
        }
    }
}