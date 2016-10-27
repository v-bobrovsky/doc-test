using Documents.Data;
using Documents.Models;
using System;

namespace Documents.Core
{
    public static class Extensions
    {
        public static UserDto ToDto(this UserViewModel model, UserDto user)
        {
            var result = user;

            if (result != null)
            {
                result.Login = model.Login;
                result.UserName = model.UserName;
            }

            return result;
        }

        public static CommentDto ToDto(this CommentViewModel model)
        {
            return new CommentDto()
            {
                Id = 0,
                CreatedTime = DateTime.Now,
                ModifiedTime = DateTime.Now,
                CanModify = true,
                Subject = model.Subject,
                Content = model.Content,
                DocumentId = model.DocumentId
            };
        }

        public static CommentDto ToDto(this CommentViewModel model, CommentDto comment)
        {
            var result = comment;

            if (result != null)
            {
                result.ModifiedTime = DateTime.Now;
                result.Subject = model.Subject;
                result.Content = model.Content;
            }

            return result;
        }

        public static DocumentDto ToDto(this DocumentViewModel model, DocumentDto document)
        {
            var result = document;

            if (result != null)
            {
                result.ModifiedTime = DateTime.Now;
                result.Name = model.Name;
                result.Content = model.Content;
            }

            return result;
        }

        public static DocumentDto ToDto(this DocumentViewModel model)
        {
            return new DocumentDto()
            {
                Id = Guid.NewGuid(),
                CreatedTime = DateTime.Now,
                ModifiedTime = DateTime.Now,
                CanModify = true,
                Name = model.Name,
                Content = model.Content
            };
        }
    }
}