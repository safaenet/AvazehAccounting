using SharedLibrary.SettingsModels.WindowsApplicationSettingsModels;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public interface IAppSettingsManager
    {
        Task<AppSettingsModel> LoadAllSettingsAsync();
        Task<bool> SaveAllSettingsAsync(AppSettingsModel settings);
        Task<object> LoadSettings(string Section);
        Task<InvoiceSettingsModel> LoadInvoiceSettings();
    }
}