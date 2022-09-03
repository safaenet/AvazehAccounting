using DataLibraryCore.DataAccess.Interfaces;
using SharedLibrary.SettingsModels.WindowsApplicationSettingsModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace DataLibraryCore.DataAccess
{
    public class AppSettingsManager : IAppSettingsManager
    {
        public AppSettingsManager()
        {
            CheckSettingsFile().ConfigureAwait(true);
        }

        private readonly string SettingsFileName = AppDomain.CurrentDomain.BaseDirectory + "appsettings.xml";

        private async Task CheckSettingsFile()
        {
            if (!File.Exists(SettingsFileName))
                await WriteSettingsFileToDisk(new AppSettingsModel());
        }

        private async Task WriteSettingsFileToDisk(AppSettingsModel settings)
        {
            if (settings == null) settings = new();
            XmlSerializer xmlSerializer = new(settings.GetType());
            StringWriter stringWriter = new();
            xmlSerializer.Serialize(stringWriter, settings);
            await File.WriteAllTextAsync(SettingsFileName, stringWriter.ToString());
        }

        public async Task<AppSettingsModel> LoadAllSettingsAsync()
        {
            await CheckSettingsFile();
            AppSettingsModel settings = new();
            XmlSerializer xmlSerializer = new(settings.GetType());
            string xmlString = await File.ReadAllTextAsync(SettingsFileName);
            StringReader stringReader = new StringReader(xmlString);
            settings = xmlSerializer.Deserialize(stringReader) as AppSettingsModel;
            return settings;
        }

        public async Task<bool> SaveAllSettingsAsync(AppSettingsModel settings)
        {
            if (settings == null) return false;
            await WriteSettingsFileToDisk(settings);
            return true;
        }

        public async Task<object> LoadSettings(string Section)
        {
            if (string.IsNullOrEmpty(Section)) return null;
            var settings = await LoadAllSettingsAsync();
            switch (Section)
            {
                case nameof(AppSettingsModel.InvoiceSettings):
                    return settings.InvoiceSettings;

                case nameof(AppSettingsModel.TransactionSettings):
                    return settings.TransactionSettings;

                case nameof(AppSettingsModel.ChequeSettings):
                    return settings.ChequeSettings;

                case nameof(AppSettingsModel.GeneralSettings):
                    return settings.GeneralSettings;

                case nameof(AppSettingsModel.InvoicePrintSettings):
                    return settings.InvoicePrintSettings;

                default: return null;
            }
        }

        public async Task<InvoiceSettingsModel> LoadInvoiceSettings()
        {
            var settings = await LoadAllSettingsAsync();
            return settings.InvoiceSettings;
        }
    }
}