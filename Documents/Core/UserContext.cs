using System;

using Documents.Data;
using Documents.Services;

namespace Documents.Core
{
    public class UserContext : IUserContext
    {
        public int GetCurrentId()
        {
            return 0;
        }

        public UserDto GetCurrentUser()
        {
            return new UserDto();
        }
    }
}