namespace SharedLibrary.SecurityAndSettingsModels;

public class UserInfoModel : UserInfoBaseModel
{
    public UserPermissionsModel Permissions { get; set; }
    public UserSettingsModel Settings { get; set; }
}