using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Documents.Models
{
    /// <summary>
    /// Represents user login data
    /// </summary>
    public class LoginViewModel
    {
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