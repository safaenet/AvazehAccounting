using AvazehApiClient.DataAccess.Interfaces;
using SharedLibrary.SecurityAndSettingsModels;
using System.Threading.Tasks;

namespace AvazehApiClient.DataAccess
{
    public class AppSettingsManager : IAppSettingsManager
    {
        public AppSettingsManager(IApiProcessor apiProcessor)
        {
            Processor = apiProcessor;
        }
        private IApiProcessor Processor;
        private readonly string Key = "AppSettings";

        public async Task<AppSettingsModel> LoadAllAppSettings() => await Processor.GetItemAsync<AppSettingsModel>(Key, null);
        public async Task SaveAllAppSettings(AppSettingsModel Settings) => await Processor.CreateItemAsync<AppSettingsModel, object>(Key, Settings);
        public async Task<GeneralSettingsModel> LoadGeneralSettings() => await Processor.GetItemAsync<GeneralSettingsModel>($"{ Key }/{ nameof(AppSettingsModel.GeneralSettings) }", null);
        public async Task<PrintSettingsModel> LoadInvoicePrintSettings() => await Processor.GetItemAsync<PrintSettingsModel>($"{ Key }/{ nameof(AppSettingsModel.PrintSettings) }", null);

    }
}