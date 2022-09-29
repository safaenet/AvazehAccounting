using AvazehApiClient.DataAccess;
using SharedLibrary.SecurityAndSettingsModels;
using System.Threading.Tasks;

namespace AvazehWpf
{
    public class AppSettings
    {
        public AppSettings()
        {
            Settings = new();
            CurrentPersianDate = new System.Globalization.PersianCalendar().GetPersianDate();
            _ = LoadSettingsAsync().ConfigureAwait(false);
        }

        private async Task LoadSettingsAsync()
        {
            AppSettingsManager manager = new(new ApiProcessor());
            Settings = await manager.LoadAllAppSettings();
        }

        private AppSettingsModel settings;

        public AppSettingsModel Settings { get => settings; set => settings = value; }
        public string CurrentPersianDate { get; init; }
    }
}