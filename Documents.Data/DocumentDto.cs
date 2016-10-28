using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documents.Data
{
    /// <summary>
    /// Dto for document object
    /// </summary>
    public class DocumentDto : BaseEntityDto<Guid>
    {
        /// <summary>
        /// Author name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Identity of author
        /// </summary>
        public int UserId { get; set; }

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