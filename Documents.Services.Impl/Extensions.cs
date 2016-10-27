using System;

using Documents.Data;
using Documents.DataAccess;

namespace Documents.Services.Impl
{
    public static class Extensions
    {
        public static User ToEntity(this UserDto dto, User entity)
        {
            var result = entity;

            if (result != null)
            {
                result.Name = dto.Login;
                result.UserName = dto.UserName;
            }

            return result;
        }

        public static User ToEntity(this UserDto dto)
        {
            return new User()
            {
                UserId = dto.UserId,
                UserName = dto.UserName,
                Name = dto.Login
            };
        }
        
        public static UserDto ToDto(this User entity)
        {
            return new UserDto()
            {
                 UserId = entity.UserId,
                 UserName = entity.UserName,
                 Login = entity.Name
            };
        }

        public static CommentDto ToDto(this Comment entity, bool canModify)
        {
            return new CommentDto()
            {
                Id = entity.Id,
                CreatedTime = entity.CreatedTime,
                ModifiedTime = entity.ModifiedTime,
                AuthorName = entity.User.Name,
                CanModify = canModify,
                Subject = entity.Subject,
                Content = entity.Content,
                DocumentId = entity.DocumentId
            };
        }

        public static Comment ToEntity(this CommentDto dto, int userId)
        {
            return new Comment()
            {
                Id = dto.Id,
                CreatedTime = DateTime.Now,
                ModifiedTime = DateTime.Now,
                UserId = userId,
                Subject = dto.Subject,
                Content = dto.Content,
                DocumentId = dto.DocumentId
            };
        }

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

        public static DocumentDto ToDto(this Document entity, bool canModify)
        {
            return new DocumentDto()
            {
                Id = entity.Id,
                CreatedTime = entity.CreatedTime,
                ModifiedTime = entity.ModifiedTime,
                AuthorName = entity.User.Name,
                CanModify = canModify,
                Name = entity.Name,
                Content = entity.Content
            };
        }

        public static Document ToEntity(this DocumentDto dto, int userId)
        {
            return new Document()
            {
                Id = dto.Id,
                CreatedTime = DateTime.Now,
                ModifiedTime = DateTime.Now,
                UserId = userId,
                Name = dto.Name,
                Content = dto.Content
            };
        }

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