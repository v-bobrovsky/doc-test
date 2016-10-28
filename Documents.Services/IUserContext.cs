using System;
using System.Collections.Generic;

using Documents.Data;

namespace Documents.Services
{
    /// <summary>
    /// Provides to get currnet user context
    /// </summary>
    public interface IUserContext
    {
        /// <summary>
        /// Retrieves a current user identifier
        /// </summary>
        int GetCurrentId();
    }
}