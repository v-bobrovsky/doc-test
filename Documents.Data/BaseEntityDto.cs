using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documents.Data
{
    /// <summary>
    /// Base class for dto entities.
    /// </summary>
    public abstract class BaseEntityDto<TIdentity>
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public TIdentity Id { get; set; }

        /// <summary>
        /// Time this object was created.
        /// </summary>
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// Last time this object was modified.
        /// </summary>
        public DateTime ModifiedTime { get; set; }

        /// <summary>
        /// User can modify this object 
        /// </summary>
        public bool CanModify { get; set; }
    }
}