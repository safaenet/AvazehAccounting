using System;

namespace SharedLibrary.SecurityAndSettingsModels;

public class LoggedInUser_DTO
{
    public int Id { get; set; }
    public string Token { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime LastLoginDate { get; set; }
    public UserSettingsModel UserSettings { get; set; }
    public PrintSettingsModel PrintSettings { get; set; }
    public GeneralSettingsModel GeneralSettings { get; set; }
}