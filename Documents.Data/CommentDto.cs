using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documents.Data
{
    /// <summary>
    /// Dto for Comment object
    /// </summary>
    public class CommentDto : BaseEntityDto<int>
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
