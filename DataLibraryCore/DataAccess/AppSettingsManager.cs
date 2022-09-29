using Dapper;
using DataLibraryCore.DataAccess.Interfaces;
using DataLibraryCore.DataAccess.SqlServer;
using SharedLibrary.DalModels;
using SharedLibrary.SecurityAndSettingsModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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
        private readonly string InsertUserDescriptionsQuery = @"DELETE FROM UserDescriptions; DECLARE @newId int; SET @newId = (SELECT ISNULL(MAX([Id]), 0) FROM [PhoneNumbers]) + 1;
            INSERT INTO UserDescriptions ([Id], DescriptionTitle, DescriptionText) VALUES (@newId, @descriptionTitle, @descriptionText)";

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
            if (settings != null && settings.PrintSettings != null) settings.PrintSettings.UserDescriptions = await GetUserDescriptionsAsync();
            if (settings == null) settings = new();
            if (settings.PrintSettings == null) settings.PrintSettings = new();
            if (settings.GeneralSettings == null) settings.GeneralSettings = new();
            return settings;
        }

        public async Task<bool> SaveAllSettingsAsync(AppSettingsModel settings)
        {
            if (settings == null) return false;
            if (settings.PrintSettings != null) await InsertUserDescriptionsToDatabaseAsync(settings.PrintSettings.UserDescriptions);
            settings.PrintSettings.UserDescriptions = null;
            await WriteSettingsFileToDisk(settings);
            return true;
        }

        public async Task<GeneralSettingsModel> LoadGeneralSettingsAsync()
        {
            var settings = await LoadAllSettingsAsync();
            return settings.GeneralSettings;
        }

        public async Task<PrintSettingsModel> LoadPrintSettingsAsync()
        {
            var settings = await LoadAllSettingsAsync();
            return settings.PrintSettings;
        }

        private async Task<List<UserDescriptionModel>> GetUserDescriptionsAsync()
        {
            var items = await DataAccess.LoadDataAsync<UserDescriptionModel, DynamicParameters>(GetUserDescriptionsQuery, null);
            return items.AsList();
        }

        private async Task<int> InsertUserDescriptionsToDatabaseAsync(List<UserDescriptionModel> descriptions)
        {
            if (descriptions != null && descriptions.Count > 0) return await DataAccess.SaveDataAsync(InsertUserDescriptionsQuery, descriptions);
            return 0;
        }
    }
}