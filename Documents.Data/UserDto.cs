﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documents.Data
{
    public class UserDto
    {
        public int UserId { get; set; }

        public string UserName { get; set; }
        
        public string Login { get; set; }

        public string UserRole { get; set; }
    }
}
