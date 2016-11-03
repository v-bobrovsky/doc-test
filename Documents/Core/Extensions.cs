using System;

using Documents.Data;
using Documents.Models;
using System.Security.Principal;
using System.Security.Claims;
using System.Globalization;

namespace Documents.Core
{
    public static class Extensions
    {
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
    }
}