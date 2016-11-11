using System;
using System.Collections.Generic;

using Documents.Data;

namespace Documents.Services
{
    /// <summary>
    /// Provides documents functionality.
    /// </summary>
    public interface IDocumentService: IRepositoryService<DocumentDto, Guid>
    {
        /// <summary>
        /// Check is document owner
        /// </summary>
        /// <param name="ctx">Contains information of current user</param>
        /// <param name="id">Document unique identifier</param>
        /// <returns></returns>
        bool CheckIsDocumentOwner(PermissionsContext ctx, Guid id);
    }
}