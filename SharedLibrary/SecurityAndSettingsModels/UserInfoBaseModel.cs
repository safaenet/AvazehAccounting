namespace SharedLibrary.SecurityAndSettingsModels
{
    public class UserInfoBaseModel
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateCreated { get; set; }
        public string LastLoginDate { get; set; }
        public string LastLoginTime { get; set; }
        public string FullName => FirstName + " " + LastName;
    }
}