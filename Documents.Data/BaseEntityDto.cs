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

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != this.GetType())
                return false;

            return Equals((BaseEntityDto<TIdentity>)obj);
        }

        protected bool Equals(BaseEntityDto<TIdentity> other)
        {
            return Id.Equals(other.Id)
                && CreatedTime.ToString() == other.CreatedTime.ToString()
                && ModifiedTime.ToString() == other.ModifiedTime.ToString()
                && CanModify == other.CanModify;
        }
    }
}