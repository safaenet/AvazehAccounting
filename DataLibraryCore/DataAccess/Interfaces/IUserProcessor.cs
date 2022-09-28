using SharedLibrary.SecurityAndSettingsModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public interface IUserProcessor
    {
        Task<List<UserInfoBase>> GetUsersList();
        Task<int> GetCountOfAdminUsers();
        Task<UserInfo> CreateUser(User_DTO_CreateUpdate user);
        Task<bool> VerifyUser(UserLogin_DTO user);
        Task<int> DeleteUser(string username);
        Task<UserInfoBase> GetUserInfoBase(UserLogin_DTO user);
        Task<UserPermissions> GetUserPermissions(UserLogin_DTO user);
        Task<UserSettings> GetUserSettings(UserLogin_DTO user);
        Task<UserInfo> UpdateUser(User_DTO_CreateUpdate user);
        Task UpdateUserLastLoginDate(string username);
    }
}