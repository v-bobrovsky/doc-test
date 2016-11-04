using Documents.Data;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Http.Description;

namespace Documents.Models
{
    /// <summary>
    /// Web api user
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ServiceUser : IUser
    {
        private UserDto _userDto;

        /// <summary>
        /// User identity
        /// </summary>
        public string Id
        {
            get 
            {
                return _userDto != null
                    ? _userDto.Id.ToString() 
                    : string.Empty; 
            }
        }

        /// <summary>
        /// User unique name (login)
        /// </summary>
        public string UserName
        {
            get
            {
                return _userDto.Login;
            }

            set {;}
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="user"></param>
        public ServiceUser(UserDto user)
        {
            _userDto = user;
        }

        /// <summary>
        /// Convert to DTO
        /// </summary>
        /// <returns></returns>
        public UserDto ToDto()
        {
            return _userDto;
        }
    }
}