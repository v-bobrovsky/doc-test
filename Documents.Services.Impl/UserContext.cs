using System;

using Documents.Data;
using Documents.DataAccess;

namespace Documents.Services.Impl
{
    public class UserContext : IUserContext
    {
        public int GetCurrentId()
        {
            return 0;
        }
    }
}