﻿using System;
using System.Collections.Generic;

using Documents.Data;

namespace Documents.Services
{
    /// <summary>
    /// Provides commentaries functionality.
    /// </summary>
    public interface ICommentService : IRepositoryService<CommentDto, int>
    {
        /// <summary>
        /// Check is comment owner
        /// </summary>
        /// <param name="ctx">Contains information of current user</param>
        /// <param name="id">Commentary unique identifier</param>
        /// <returns></returns>
        bool CheckIsCommentOwner(PermissionsContext ctx, int id);
    }
}