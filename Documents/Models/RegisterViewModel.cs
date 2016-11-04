using System;
using System.ComponentModel.DataAnnotations;

namespace Documents.Models
{
    /// <summary>
    /// Represents register data for new user
    /// </summary>
    public class RegisterViewModel
    {
        /// <summary>
        /// User Name
        /// </summary>
        [Required]
        [Display(Name = "User name")]
        [StringLength(150, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        [DataType(DataType.Text)]
        public string UserName { get; set; }

        /// <summary>
        /// User Login
        /// </summary>
        [Required]
        [Display(Name = "Login")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        public string Login { get; set; }

        /// <summary>
        /// User Password
        /// </summary>
        [Required]
        [Display(Name = "Password")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}