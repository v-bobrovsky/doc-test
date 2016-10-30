using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Documents.Models
{
    public class DocumentViewModel
    {
        /// <summary>
        /// Document name
        /// </summary>
        [Required]
        [StringLength(150, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        /// <summary>
        /// Document content
        /// </summary>
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
    }
}