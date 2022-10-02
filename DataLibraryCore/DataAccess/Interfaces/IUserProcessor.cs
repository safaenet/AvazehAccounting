using SharedLibrary.SecurityAndSettingsModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public interface IUserProcessor
    {
        Task<bool> TestDBConnectionAsync();
        Task<List<UserInfoBaseModel>> GetUsersList();
        Task<int> GetCountOfAdminUsers();
        Task<UserInfoBaseModel> CreateUser(User_DTO_CreateUpdate user);
        Task<bool> VerifyUser(UserLogin_DTO user);
        Task<int> DeleteUser(int Id);
        Task<UserInfoBaseModel> GetUserInfoBase(string Username);
        Task<UserPermissionsModel> GetUserPermissions(int Id);
        Task<UserSettingsModel> GetUserSettings(int Id);
        Task<UserInfoBaseModel> UpdateUser(User_DTO_CreateUpdate user);
        Task<bool> UpdateUserSettings(string Username, UserSettingsModel userSettings);
        Task UpdateUserLastLoginDate(string username);
    }
}