using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.SecurityAndSettingsModels
{
    public class LoggedInUser_DTO
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateCreated { get; set; }
        public string LastLoginDate { get; set; }
        public UserPermissions Permissions { get; set; }
        public UserSettings Settings { get; set; }
    }
}