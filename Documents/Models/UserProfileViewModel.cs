using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Documents.Models
{
    /// <summary>
    /// Represents user profile information
    /// </summary>
    public class UserProfileViewModel
    {
        /// <summary>
        /// User Name
        /// </summary>
        [Required]
        [StringLength(150, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [DataType(DataType.Text)]
        public string UserName { get; set; }

        /// <summary>
        /// User Login
        /// </summary>
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        public string Login { get; set; }

        /// <summary>
        /// User Password
        /// </summary>
        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}