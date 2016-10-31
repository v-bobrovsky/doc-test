using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documents.Data
{
    public class UserDto : BaseEntityDto<int>
    {
        public string UserName { get; set; }
        
        public string Login { get; set; }

        public string Password { get; set; }

        public string UserRole { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != this.GetType())
                return false;

            return Equals((UserDto)obj);
        }

        protected bool Equals(UserDto other)
        {
            return base.Equals(other)
                && UserName == other.UserName
                && Login == other.Login
                && Password == other.Password               
                && UserRole == other.UserRole;
        }
    }
}
