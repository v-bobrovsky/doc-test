using System;
using System.Collections.Generic;

using Documents.Data;

namespace Documents.Services
{
    /// <summary>
    /// Provides users functionality.
    /// </summary>
    public interface IUserService : IRepositoryService<UserDto, int>
    {
    }
}