using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documents.Data
{
    /// <summary>
    /// Represents information of current user
    /// </summary>
    public class PermissionsContext
    {
        /// <summary>
        /// Current user identity
        /// </summary>
        public int CurrentUserId { get; protected set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="currentUserId"></param>
        public PermissionsContext(int currentUserId)
        {
            CurrentUserId = currentUserId;
        }
    }
}
