using Documents.Data;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Documents.Models
{
    /// <summary>
    /// Web api user
    /// </summary>
    public class ServiceUser : IUser
    {
        private UserDto _userDto;

        public string Id
        {
            get 
            {
                return _userDto != null
                    ? _userDto.Id.ToString() 
                    : string.Empty; 
            }
        }

        public string UserName
        {
            get
            {
                return _userDto.Login;
            }

            set {;}
        }

        public ServiceUser(UserDto user)
        {
            _userDto = user;
        }

        public UserDto ToDto()
        {
            return _userDto;
        }
    }
}