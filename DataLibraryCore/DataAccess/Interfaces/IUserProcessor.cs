using SharedLibrary.SecurityAndSettingsModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces;

public interface IUserProcessor
{
    Task<bool> TestDBConnectionAsync();
    Task<IEnumerable<UserInfoBaseModel>> GetUsersAsync();
    Task<int> GetCountOfAdminUsersAsync();
    Task<UserInfoBaseModel> CreateUserAsync(User_DTO_CreateUpdate user);
    Task<bool> VerifyUserAsync(UserLogin_DTO user);
    Task<int> DeleteUserAsync(int Id);
    Task<UserInfoBaseModel> GetUserInfoBaseAsync(string Username);
    Task<UserPermissionsModel> GetUserPermissionsAsync(int Id);
    Task<UserSettingsModel> GetUserSettingsAsync(int Id);
    Task<UserInfoBaseModel> UpdateUserInfoAsync(UserInfoBaseModel user, bool ChangePassword = false, string NewPassword = null);
    Task<UserPermissionsModel> UpdateUserPermissionsAsync(int Id, UserPermissionsModel userPermissions);
    Task<UserSettingsModel> UpdateUserSettingsAsync(int Id, UserSettingsModel userSettings);
    Task UpdateUserLastLoginDateAsync(string username);
}