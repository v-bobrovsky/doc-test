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
        /// <param name="id">Document unique identifier</param>
        /// <returns></returns>
        bool CheckIsDocumentOwner(Guid id);
    }
}