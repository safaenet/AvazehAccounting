using SharedLibrary.Security;
using SharedLibrary.Settings;

namespace SharedLibrary.Models;

public class UserInfoModel : UserInfoBaseModel
{
    public UserPermissionsModel Permissions { get; set; }
    public UserSettingsModel Settings { get; set; }
}