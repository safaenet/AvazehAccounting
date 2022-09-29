using SharedLibrary.SecurityAndSettingsModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public interface IUserProcessor
    {
        Task<List<UserInfoBaseModel>> GetUsersList();
        Task<int> GetCountOfAdminUsers();
        Task<UserInfoBaseModel> CreateUser(User_DTO_CreateUpdate user);
        Task<bool> VerifyUser(UserLogin_DTO user);
        Task<int> DeleteUser(string username);
        Task<UserInfoBaseModel> GetUserInfoBase(UserLogin_DTO user);
        Task<UserPermissionsModel> GetUserPermissions(UserLogin_DTO user);
        Task<UserSettingsModel> GetUserSettings(UserLogin_DTO user);
        Task<UserInfoModel> UpdateUser(User_DTO_CreateUpdate user);
        Task UpdateUserLastLoginDate(string username);
    }
}