using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.SecurityAndSettings
{
    public class User_DTO_CreateUpdate
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserPermissions Permissions { get; set; }
        public UserSettings Settings { get; set; }
    }
}