using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Documents.Models
{
    public class CommentViewModel
    {
        /// <summary>
        /// Subject for comment
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// General content for comment
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Document unique identifier
        /// </summary>
        public Guid DocumentId { get; set; }
    }
}