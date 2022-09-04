using AvazehApiClient.DataAccess;
using SharedLibrary.SettingsModels.WindowsApplicationSettingsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvazehWpf
{
    public class AppSettings
    {
        public AppSettings()
        {
            Settings = new();
            CurrentPersianDate = new System.Globalization.PersianCalendar().GetPersianDate();
            LoadSettings().ConfigureAwait(false);
        }

        private async Task LoadSettings()
        {
            AppSettingsManager manager = new(new ApiProcessor());
            Settings = await manager.LoadAllAppSettings();
        }

        private AppSettingsModel settings;

        public AppSettingsModel Settings { get => settings; set => settings = value; }
        public string CurrentPersianDate { get; init; }
    }
}