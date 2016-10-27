using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Documents.Models
{
    public class DocumentViewModel
    {
        /// <summary>
        /// Document name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Document content
        /// </summary>
        public string Content { get; set; }
    }
}