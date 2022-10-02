using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.SecurityAndSettingsModels
{
    public class UserInfoModel : UserInfoBaseModel
    {
        public UserPermissionsModel Permissions { get; set; }
        public UserSettingsModel Settings { get; set; }
    }
}