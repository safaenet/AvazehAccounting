namespace SharedLibrary.SecurityAndSettingsModels;

public class User_DTO_CreateUpdate
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool IsActive { get; set; }
    public UserPermissionsModel Permissions { get; set; }
    public UserSettingsModel Settings { get; set; }
}