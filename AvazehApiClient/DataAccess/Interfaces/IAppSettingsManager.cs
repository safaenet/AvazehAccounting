using SharedLibrary.SettingsModels;
using SharedLibrary.SettingsModels.WindowsApplicationSettingsModels;
using System.Threading.Tasks;

namespace AvazehApiClient.DataAccess.Interfaces
{
    public interface IAppSettingsManager
    {
        Task<AppSettingsModel> LoadAllAppSettings();
        Task<ChequeSettingsModel> LoadChequeSettings();
        Task<GeneralSettingsModel> LoadGeneralSettings();
        Task<PrintSettingsModel> LoadInvoicePrintSettings();
        Task<InvoiceSettingsModel> LoadInvoiceSettings();
        Task<TransactionSettingsModel> LoadTransactionSettings();
        Task SaveAllAppSettings(AppSettingsModel Settings);
    }
}