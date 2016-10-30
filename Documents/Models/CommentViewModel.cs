using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Documents.Models
{
    public class CommentViewModel
    {
        /// <summary>
        /// Subject for comment
        /// </summary>
        [StringLength(150, ErrorMessage = "The {0} must be at least {2} characters long.")]
        [DataType(DataType.Text)]
        public string Subject { get; set; }

        /// <summary>
        /// Commentary text
        /// </summary>
        [Required]
        [StringLength(500, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }

        /// <summary>
        /// Document unique identifier
        /// </summary>
        public Guid DocumentId { get; set; }
    }
}