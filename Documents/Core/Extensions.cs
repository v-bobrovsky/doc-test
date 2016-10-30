using Documents.Data;
using Documents.Models;
using System;

namespace Documents.Core
{
    public static class Extensions
    {
        public static UserDto ToDto(this UserViewModel model)
        {
            return new UserDto()
            {
                Login = model.Login,
                UserName = model.UserName,
                Password = model.Password
            };
        }

        public static CommentDto ToDto(this CommentViewModel model)
        {
            return new CommentDto()
            {
                Subject = model.Subject,
                Content = model.Text,
                DocumentId = model.DocumentId
            };
        }

        public static DocumentDto ToDto(this DocumentViewModel model)
        {
            return new DocumentDto()
            {
                Name = model.Name,
                Content = model.Content
            };
        }
    }
}