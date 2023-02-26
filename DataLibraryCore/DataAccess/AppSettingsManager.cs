using Dapper;
using DataLibraryCore.DataAccess.Interfaces;
using DataLibraryCore.DataAccess.SqlServer;
using Serilog;
using SharedLibrary.DalModels;
using SharedLibrary.SecurityAndSettingsModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace DataLibraryCore.DataAccess;

public class AppSettingsManager : IAppSettingsManager
{
    public AppSettingsManager()
    {
        CheckSettingsFile().ConfigureAwait(true);
        DataAccess = new SqlDataAccess();
    }

    private static string SettingsFileName = AppDomain.CurrentDomain.BaseDirectory + "appsettings.xml";
    private readonly IDataAccess DataAccess;
    private readonly string GetUserDescriptionsQuery = "SELECT [Id], [DescriptionTitle], [DescriptionText] From UserDescriptions";
    private readonly string GetProductUnitsQuery = "SELECT [Id], [UnitName] From ProductUnits";
    private readonly string DeleteAllUserDescriptionsCmd = "DELETE FROM UserDescriptions;";
    private readonly string InsertUserDescriptionsQuery = @"INSERT INTO UserDescriptions ([Id], DescriptionTitle, DescriptionText) VALUES (@id, @descriptionTitle, @descriptionText)";
    private readonly string InsertProductUnits = @"DECLARE @newId int; SET @newId = (SELECT ISNULL(MAX([Id]), 0) FROM [ProductUnits]) + 1;
            INSERT INTO ProductUnits ([Id], [UnitName]) VALUES (@newId, @unitName)";
    private readonly string UpdateProductUnits = "Update ProductUnits SET [UnitName] = @unitName WHERE [Id] = @id";
    private readonly string DeleteProductUnits = "Delete ProductUnits WHERE [Id] = @id";

    private async Task CheckSettingsFile()
    {
        if (!File.Exists(SettingsFileName))
            await WriteSettingsFileToDisk(new AppSettingsModel());
    }

    private static async Task WriteSettingsFileToDisk(AppSettingsModel settings)
    {
        try
        {
            if (settings == null) settings = new();
            XmlSerializer xmlSerializer = new(settings.GetType());
            StringWriter stringWriter = new();
            xmlSerializer.Serialize(stringWriter, settings);
            await File.WriteAllTextAsync(SettingsFileName, stringWriter.ToString());

            //if (settings == null) settings = new();
            //XmlWriterSettings xmlWriterSettings = new()
            //{
            //    Indent = true,
            //    Async = true
            //};
            //XmlSerializer xmlSerializer = new(settings.GetType());
            //StringWriter stringWriter = new();
            //XmlWriter xmlWriter = XmlWriter.Create(stringWriter, xmlWriterSettings);

            //xmlSerializer.Serialize(xmlWriter, settings);
            //await xmlWriter.FlushAsync();
            //xmlWriter.Close();
            //await File.WriteAllTextAsync(SettingsFileName, xmlWriter);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in AppSettingsManager");
        }
    }

    public async Task<AppSettingsModel> LoadAllSettingsAsync()
    {
        try
        {
            await CheckSettingsFile();
            AppSettingsModel settings = new();
            XmlSerializer xmlSerializer = new(settings.GetType());
            string xmlString = await File.ReadAllTextAsync(SettingsFileName);
            StringReader stringReader = new StringReader(xmlString);
            settings = xmlSerializer.Deserialize(stringReader) as AppSettingsModel;
            if (settings.PrintSettings == null) settings.PrintSettings = new();
            if (settings.GeneralSettings == null) settings.GeneralSettings = new();
            if (settings != null && settings.PrintSettings != null) settings.PrintSettings.UserDescriptions = await GetUserDescriptionsAsync();
            if (settings != null && settings.GeneralSettings != null) settings.GeneralSettings.ProductUnits = await GetProductUnitsAsync();
            if (settings == null) settings = new();
            return settings;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in AppSettingsManager");
        }
        return null;
    }

    public async Task<bool> SaveAllSettingsAsync(AppSettingsModel settings)
    {
        try
        {
            if (settings == null) return false;
            if (settings.PrintSettings != null) await InsertUserDescriptionsToDatabaseAsync(settings.PrintSettings.UserDescriptions);
            if (settings.GeneralSettings != null) await InsertProductUnitsToDatabaseAsync(settings.GeneralSettings.ProductUnits);
            settings.PrintSettings.UserDescriptions = null;
            settings.GeneralSettings.ProductUnits = null;
            await WriteSettingsFileToDisk(settings);
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in AppSettingsManager");
        }
        return false;
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

    private async Task<List<ProductUnitModel>> GetProductUnitsAsync()
    {
        var items = await DataAccess.LoadDataAsync<ProductUnitModel, DynamicParameters>(GetProductUnitsQuery, null);
        return items.AsList();
    }

    private async Task<int> InsertUserDescriptionsToDatabaseAsync(List<UserDescriptionModel> descriptions)
    {
        if (descriptions != null && descriptions.Count > 0)
        {
            _ = await DataAccess.SaveDataAsync<DynamicParameters>(DeleteAllUserDescriptionsCmd, null);
            int id = 1;
            foreach (var item in descriptions)
                item.Id = id++;
            return await DataAccess.SaveDataAsync(InsertUserDescriptionsQuery, descriptions);
        }
        return 0;
    }

    private async Task<bool> InsertProductUnitsToDatabaseAsync(List<ProductUnitModel> units)
    {
        try
        {
            var backup_units = await GetProductUnitsAsync();
            if (units == null || units.Count == 0) return false;
            var updatables = units.Where(productUnit => productUnit.Id != -1).ToList();
            var newUnits = units.Where(productUnit => productUnit.Id == -1).ToList();
            var deletedUnits = backup_units.Except(units).ToList();

            if (newUnits != null && newUnits.Count > 0) await DataAccess.SaveDataAsync(InsertProductUnits, newUnits);
            if (updatables != null && updatables.Count > 0) await DataAccess.SaveDataAsync(UpdateProductUnits, units);
            if (deletedUnits != null && deletedUnits.Count > 0) await DataAccess.SaveDataAsync(DeleteProductUnits, deletedUnits);
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in AppSettingsManager");
        }
        return false;
    }
}