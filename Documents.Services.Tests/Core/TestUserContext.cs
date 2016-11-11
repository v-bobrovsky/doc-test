using Documents.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documents.Services.Tests.Core
{
    public class TestUserContext
    {
        private static UserDto _user;

        /// <summary>
        /// Constructor
        /// </summary>
        public TestUserContext()
        {
            _user = null;
        }

        public void SetUser(UserDto user)
        {
            _user = user;
        }

        public int GetUserId()
        {
            return _user != null 
                ? _user.Id : 0;
        }

        public UserDto GetUser()
        {
            return _user;
        }

        public PermissionsContext GetPermissionsCtx()
        {
             var userId = GetUserId();
             return new PermissionsContext(userId);
        }
    }
}