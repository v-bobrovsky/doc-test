using Documents.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documents.Services.Tests.Core
{
    public class TestUserContext: IUserContext
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

        public int GetCurrentId()
        {
            return _user != null ? _user.Id : 0;
        }

        public UserDto GetCurrentUser()
        {
            return _user;
        }
    }
}