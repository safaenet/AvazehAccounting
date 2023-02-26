using System;

namespace SharedLibrary.SecurityAndSettingsModels;

public class UserInfoBaseModel
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime LastLoginDate { get; set; }
    public bool IsActive { get; set; } = true;
    public string FullName => FirstName + " " + LastName;
}