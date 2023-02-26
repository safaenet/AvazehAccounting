using SharedLibrary.SecurityAndSettingsModels;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces;

public interface IAppSettingsManager
{
    Task<AppSettingsModel> LoadAllSettingsAsync();
    Task<bool> SaveAllSettingsAsync(AppSettingsModel settings);
    Task<GeneralSettingsModel> LoadGeneralSettingsAsync();
    Task<PrintSettingsModel> LoadPrintSettingsAsync();
}