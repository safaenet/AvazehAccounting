using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.SecurityAndSettingsModels
{
    public class UserInfoBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateCreated { get; set; }
        public string LastLoginDate { get; set; }
    }
}