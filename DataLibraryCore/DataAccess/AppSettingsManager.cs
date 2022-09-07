using Dapper;
using DataLibraryCore.DataAccess.Interfaces;
using DataLibraryCore.DataAccess.SqlServer;
using SharedLibrary.DalModels;
using SharedLibrary.SettingsModels;
using SharedLibrary.SettingsModels.WindowsApplicationSettingsModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            DataAccess = new SqlDataAccess();
        }

        private readonly string SettingsFileName = AppDomain.CurrentDomain.BaseDirectory + "appsettings.xml";
        private readonly IDataAccess DataAccess;
        private readonly string GetUserDescriptionsQuery = "SELECT [Id], [DescriptionTitle], [DescriptionText] From UserDescriptions";
        private readonly string DeleteUserDescriptionsQuery = "DELETE FROM UserDescriptions";
        private readonly string InsertUserDescriptionsQuery = "INSERT INTO UserDescriptions (DescriptionTitle, DescriptionText) VALUES (@descriptionTitle, @descriptionText)";

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
            if (settings != null && settings.InvoicePrintSettings != null) settings.InvoicePrintSettings.UserDescriptions = await GetUserDescriptionsAsync();
            return settings;
        }

        public async Task<bool> SaveAllSettingsAsync(AppSettingsModel settings)
        {
            if (settings == null) return false;
            if (settings.InvoicePrintSettings != null) await InsertUserDescriptionsToDatabaseAsync(settings.InvoicePrintSettings.UserDescriptions);
            settings.InvoicePrintSettings.UserDescriptions = null;
            await WriteSettingsFileToDisk(settings);
            return true;
        }

        public async Task<InvoiceSettingsModel> LoadInvoiceSettings()
        {
            var settings = await LoadAllSettingsAsync();
            return settings.InvoiceSettings;
        }

        public async Task<TransactionSettingsModel> LoadTransactionSettings()
        {
            var settings = await LoadAllSettingsAsync();
            return settings.TransactionSettings;
        }

        public async Task<ChequeSettingsModel> LoadChequeSettings()
        {
            var settings = await LoadAllSettingsAsync();
            return settings.ChequeSettings;
        }

        public async Task<GeneralSettingsModel> LoadGeneralSettings()
        {
            var settings = await LoadAllSettingsAsync();
            return settings.GeneralSettings;
        }

        public async Task<InvoicePrintSettingsModel> LoadInvoicePrintSettings()
        {
            var settings = await LoadAllSettingsAsync();
            return settings.InvoicePrintSettings;
        }

        private async Task<List<UserDescriptionModel>> GetUserDescriptionsAsync()
        {
            var items = await DataAccess.LoadDataAsync<UserDescriptionModel, DynamicParameters>(GetUserDescriptionsQuery, null);
            return items.AsList();
        }

        private async Task<int> InsertUserDescriptionsToDatabaseAsync(List<UserDescriptionModel> descriptions)
        {
            await DataAccess.SaveDataAsync<DynamicParameters>(DeleteUserDescriptionsQuery, null);
            if (descriptions != null && descriptions.Count != 0) return await DataAccess.SaveDataAsync(InsertUserDescriptionsQuery, descriptions);
            return 0;
        }
    }
}