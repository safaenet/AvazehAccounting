using SharedLibrary.SecurityAndSettingsModels;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public interface IAuthProcessor
    {
        Task<UserInfo> CreateUser(User_DTO_CreateUpdate user);
        Task<int> DeleteUser(string username);
        Task<string> GetUserByCredencials(UserLogin_DTO user);
        Task<UserInfo> UpdateUser(User_DTO_CreateUpdate user);
        Task<bool> VerifyUser(UserLogin_DTO user);
    }
}