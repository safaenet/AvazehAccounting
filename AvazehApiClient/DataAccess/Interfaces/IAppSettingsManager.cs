using SharedLibrary.SecurityAndSettingsModels;
using System.Threading.Tasks;

namespace AvazehApiClient.DataAccess.Interfaces;

public interface IAppSettingsManager
{
    Task<AppSettingsModel> LoadAllAppSettings();
    Task<GeneralSettingsModel> LoadGeneralSettings();
    Task<PrintSettingsModel> LoadInvoicePrintSettings();
    Task SaveAllAppSettings(AppSettingsModel Settings);
}