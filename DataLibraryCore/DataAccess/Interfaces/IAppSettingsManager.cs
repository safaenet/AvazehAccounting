using SharedLibrary.SettingsModels;
using SharedLibrary.SettingsModels.WindowsApplicationSettingsModels;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public interface IAppSettingsManager
    {
        Task<AppSettingsModel> LoadAllSettingsAsync();
        Task<bool> SaveAllSettingsAsync(AppSettingsModel settings);
        Task<InvoiceSettingsModel> LoadInvoiceSettings();
        Task<TransactionSettingsModel> LoadTransactionSettings();
        Task<ChequeSettingsModel> LoadChequeSettings();
        Task<GeneralSettingsModel> LoadGeneralSettings();
        Task<PrintSettingsModel> LoadInvoicePrintSettings();
    }
}