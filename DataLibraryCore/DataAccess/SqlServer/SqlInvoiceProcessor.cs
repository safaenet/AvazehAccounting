﻿using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using FluentValidation.Results;
using Dapper.FluentMap.Mapping;
using Dapper.FluentMap;
using DataLibraryCore.DataAccess.Interfaces;
using SharedLibrary.Enums;
using SharedLibrary.DalModels;
using SharedLibrary.Validators;
using SharedLibrary.DtoModels;
using System.Threading.Tasks;
using System;
using Serilog;
using SharedLibrary.Helpers;

namespace DataLibraryCore.DataAccess.SqlServer;

public class SqlInvoiceProcessor : IInvoiceProcessor
{
    public SqlInvoiceProcessor(IDataAccess dataAcess)
    {
        DataAccess = dataAcess;
        if (FluentMapper.EntityMaps.IsEmpty)
        {
            try
            {
                FluentMapper.Initialize(config => config.AddMap(new CustomerModelMapper()));
                FluentMapper.Initialize(config => config.AddMap(new ProductModelMapper()));
                FluentMapper.Initialize(config => config.AddMap(new ProductUnitModelMapper()));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in SqlInvoiceProcessor");
            }
        }
    }

    private readonly IDataAccess DataAccess;
    private readonly string CreateInvoiceQuery = @"DECLARE @newId int; SET @newId = (SELECT ISNULL(MAX([Id]), 1) FROM [Invoices]) + 1;
            INSERT INTO Invoices ([Id], CustomerId, About, DateCreated, TimeCreated, DiscountType, DiscountValue, Descriptions, LifeStatus)
            VALUES (@newId, @customerId, @about, @dateCreated, @timeCreated, @discountType, @discountValue, @descriptions, @lifeStatus);
            SELECT @id = @newId;";
    private readonly string UpdateInvoiceQuery = @"UPDATE Invoices SET CustomerId = @customerId, About = @about, DateCreated = @dateCreated, TimeCreated = @timeCreated,
            DateUpdated = @dateUpdated, TimeUpdated = @timeUpdated, DiscountType = @discountType,
            DiscountValue = @discountValue, Descriptions = @descriptions, LifeStatus = @lifeStatus WHERE Id = @id";
    private readonly string InsertInvoiceItemQuery = @"DECLARE @newId int; SET @newId = (SELECT ISNULL(MAX([Id]), 0) FROM [InvoiceItems]) + 1;
            INSERT INTO InvoiceItems ([Id], InvoiceId, ProductId, BuyPrice, SellPrice, CountString, CountValue, ProductUnitId, DateCreated, TimeCreated, Delivered, Descriptions)
            VALUES (@newId, @invoiceId, @productId, @buyPrice, @sellPrice, @countString, @countValue, @productUnitId, @dateCreated, @timeCreated, @delivered, @descriptions);
            SELECT @id = @newId;";
    private readonly string UpdateInvoiceItemQuery = @$"UPDATE InvoiceItems SET ProductId = @productId, BuyPrice = @buyPrice, SellPrice = @sellPrice, CountString = @countString, 
            CountValue = @countValue, ProductUnitId = @productUnitId, DateUpdated = @dateUpdated, TimeUpdated = @timeUpdated, Delivered = @delivered, Descriptions = @descriptions WHERE [Id] = @id";
    private readonly string UpdateSubItemDateAndTimeQuery = @"UPDATE Invoices SET DateUpdated = @dateUpdated, TimeUpdated = @timeUpdated WHERE [Id] = @id";
    private readonly string DeleteInvoiceItemQuery = @$"DELETE FROM InvoiceItems WHERE [Id] = @id";
    private readonly string InsertInvoicePaymentQuery = @$"DECLARE @newId int; SET @newId = (SELECT ISNULL(MAX([Id]), 0) FROM [InvoicePayments]) + 1;
            INSERT INTO InvoicePayments ([Id], InvoiceId, DateCreated, TimeCreated, PayAmount, Descriptions)
            VALUES (@newId, @invoiceId, @dateCreated, @timeCreated, @payAmount, @descriptions);
            SELECT @id = @newId;";
    private readonly string UpdateInvoicePaymentQuery = @$"UPDATE InvoicePayments SET DateUpdated = @dateUpdated, TimeUpdated = @timeUpdated, PayAmount = @payAmount, Descriptions = @descriptions
             WHERE [Id] = @id";
    private readonly string DeleteInvoicePaymentQuery = @$"DELETE FROM InvoicePayments WHERE [Id] = @id";
    private readonly string GetSingleInvoiceItemQuery = @"SELECT it.*, p.[Id] AS pId, p.[ProductName], p.[BuyPrice] AS pBuyPrice, p.[SellPrice] AS pSellPrice, p.[Barcode],
                p.[CountString] AS pCountString, p.[DateCreated] AS pDateCreated, p.[TimeCreated] AS pTimeCreated, p.[DateUpdated] AS pDateUpdated, p.[TimeUpdated] AS pTimeUpdated,
                p.[Descriptions] AS pDescriptions, u.Id AS puId, u.UnitName
                FROM InvoiceItems it LEFT JOIN Products p ON it.ProductId = p.Id LEFT JOIN ProductUnits u ON it.ProductUnitId = u.Id WHERE it.Id = {0}";
    private readonly string GetProductItemsQuery = "SELECT [Id], [ProductName] AS ItemName FROM Products {0} ORDER BY [ProductName]";
    private readonly string GetProductUnitsQuery = "SELECT [Id], [UnitName] FROM ProductUnits";
    private readonly string GetCustomerNamesQuery = "SELECT [Id], ISNULL(FirstName, '') + ' ' + ISNULL(LastName, '') AS ItemName FROM Customers {0} ORDER BY [FirstName], [LastName]";
    private readonly string GetInvoiceAboutsQuery = "SELECT DISTINCT 0 AS Id, [About] AS ItemName FROM Invoices WHERE TRIM([About]) <> '' {0} ORDER BY [About]"; //NOT TESTED
    private readonly string GetRecentPricesOfProductQuery = @"SELECT TOP({0}) it.SellPrice AS SellPrice, it.DateCreated AS DateSold FROM InvoiceItems it LEFT JOIN Invoices i ON it.InvoiceId = i.Id
                                                             LEFT JOIN Customers c ON i.CustomerId = c.Id LEFT JOIN Products p ON it.ProductId = p.Id
                                                             WHERE c.Id = {1} AND p.Id = {2} ORDER BY DateSold DESC";

    private async Task<int> GetInvoiceIdFromInvoiceItemId(int Id)
    {
        try
        {
            var query = $"SELECT InvoiceId FROM InvoiceItems WHERE Id = { Id }";
            return await DataAccess.ExecuteScalarAsync<int>(query);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
        return 0;
    }

    private async Task<int> GetInvoiceIdFromInvoicePaymentId(int Id)
    {
        try
        {
            var query = $"SELECT InvoiceId FROM InvoicePayments WHERE Id = { Id }";
            return await DataAccess.ExecuteScalarAsync<int>(query);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
        return 0;
    }

    public string GenerateWhereClause(string val, InvoiceLifeStatus? LifeStatus, InvoiceFinancialStatus? FinStatus, SqlSearchMode mode = SqlSearchMode.OR)
    {
        try
        {
            string finStatusOperand = "";
            switch (FinStatus)
            {
                case InvoiceFinancialStatus.Balanced:
                    finStatusOperand = "=";
                    break;
                case InvoiceFinancialStatus.Creditor:
                    finStatusOperand = "<";
                    break;
                case InvoiceFinancialStatus.Deptor:
                    finStatusOperand = ">";
                    break;
                default:
                    break;
            }
            string criteria = string.IsNullOrWhiteSpace(val) ? "'%'" : $"'%{ val }%'";
            return @$"(CAST(i.[Id] AS VARCHAR) LIKE { criteria }
                      {mode} CAST(i.[CustomerId] AS VARCHAR) LIKE { criteria }
                      {mode} i.[DateCreated] LIKE { criteria }
                      {mode} i.[TimeCreated] LIKE { criteria }
                      {mode} i.[DateUpdated] LIKE { criteria }
                      {mode} i.[TimeUpdated] LIKE { criteria }
                      {mode} CAST(i.[DiscountValue] AS VARCHAR) LIKE { criteria }
                      {mode} i.[Descriptions] LIKE N{ criteria }

                      {mode} CAST(c.[Id] AS VARCHAR) LIKE { criteria }
                      {mode} ISNULL(c.FirstName, '') + ' ' + ISNULL(c.LastName, '') LIKE N{ criteria }
                      {mode} c.[CompanyName] LIKE N{ criteria }
                      {mode} c.[EmailAddress] LIKE N{ criteria }
                      {mode} c.[PostAddress] LIKE N{ criteria }
                      {mode} c.[DateJoined] LIKE { criteria }
                      {mode} c.[Descriptions] LIKE N{ criteria }

                      {mode} CAST(sp.[TotalSellValue] AS varchar) LIKE { criteria }
                      {mode} CAST(pays.[TotalPayments] AS varchar) LIKE { criteria }
                      {mode} CAST(ISNULL(dbo.CalculateDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue), 0) - ISNULL(pays.[TotalPayments], 0) AS varchar) LIKE { criteria }
                      {mode} CAST(ISNULL(dbo.CalculateDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue), 0) AS varchar) LIKE { criteria } )
                      {(LifeStatus == null ? "" : $" AND i.[LifeStatus] = { (int)LifeStatus } ")}
                      {(FinStatus == null ? "" : $" AND ISNULL(dbo.CalculateDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue), 0) - ISNULL(pays.TotalPayments, 0) { finStatusOperand } 0 ")}";
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
        return null;
    }

    public async Task<int> CreateItemAsync(InvoiceModel item)
    {
        try
        {
            if (item == null || !ValidateItem(item).IsValid) return 0;
            item.DateCreated = PersianCalendarHelper.GetCurrentPersianDate();
            item.TimeCreated = PersianCalendarHelper.GetCurrentTime();
            var dp = new DynamicParameters();
            dp.Add("@id", 0, DbType.Int32, ParameterDirection.Output);
            dp.Add("@customerId", item.Customer.Id);
            dp.Add("@about", item.About);
            dp.Add("@dateCreated", item.DateCreated);
            dp.Add("@timeCreated", item.TimeCreated);
            dp.Add("@discountType", item.DiscountType);
            dp.Add("@discountValue", item.DiscountValue);
            dp.Add("@descriptions", item.Descriptions);
            dp.Add("@lifeStatus", item.LifeStatus);
            var AffectedCount = await DataAccess.SaveDataAsync(CreateInvoiceQuery, dp);
            var OutputId = dp.Get<int>("@id");
            if (AffectedCount > 0) item.Id = OutputId;
            return OutputId;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
        return 0;
    }

    public async Task<int> UpdateItemAsync(InvoiceModel item)
    {
        try
        {
            if (item == null || !ValidateItem(item).IsValid) return 0;
            var dp = new DynamicParameters();
            dp.Add("@id", item.Id);
            dp.Add("@customerId", item.Customer.Id);
            dp.Add("@about", item.About);
            dp.Add("@dateCreated", item.DateCreated);
            dp.Add("@timeCreated", item.TimeCreated);
            dp.Add("@dateUpdated", PersianCalendarHelper.GetCurrentPersianDate());
            dp.Add("@timeUpdated", PersianCalendarHelper.GetCurrentTime());
            dp.Add("@discountType", item.DiscountType);
            dp.Add("@discountValue", item.DiscountValue);
            dp.Add("@descriptions", item.Descriptions);
            dp.Add("@lifeStatus", item.LifeStatus);
            return await DataAccess.SaveDataAsync(UpdateInvoiceQuery, dp);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
        return 0;
    }

    public async Task<int> DeleteItemByIdAsync(int Id)
    {
        try
        {
            string sql = @$"DELETE FROM Invoices WHERE Id = {Id}";
            return await DataAccess.SaveDataAsync(sql);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
        return 0;
    }

    public async Task<InvoiceItemModel> GetInvoiceItemFromDatabaseAsync(int Id)
    {
        try
        {
            var sql = string.Format(GetSingleInvoiceItemQuery, Id);
            using IDbConnection conn = new SqlConnection(DataAccess.GetConnectionString());
            var result = await conn.QueryAsync<InvoiceItemModel, ProductModel, ProductUnitModel, InvoiceItemModel>
                (sql, (it, p, u) => { it.Product = p; it.Unit = u; return it; }, splitOn: "pId, puId");
            return result.SingleOrDefault();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
        return null;
    }

    public async Task<int> InsertInvoiceItemToDatabaseAsync(InvoiceItemModel item)
    {
        try
        {
            if (item == null || !item.IsCountStringValid) return 0;
            item.DateCreated = PersianCalendarHelper.GetCurrentPersianDate();
            item.TimeCreated = PersianCalendarHelper.GetCurrentTime();
            DynamicParameters dp = new();
            dp.Add("@id", 0, DbType.Int32, ParameterDirection.Output);
            dp.Add("@invoiceId", item.InvoiceId);
            dp.Add("@productId", item.Product.Id);
            dp.Add("@buyPrice", item.BuyPrice);
            dp.Add("@sellPrice", item.SellPrice);
            dp.Add("@countString", item.CountString);
            dp.Add("@countValue", decimal.Round(item.CountValue, 3));
            dp.Add("@productUnitId", item.Unit == null ? (int?)null : item.Unit.Id);
            dp.Add("@dateCreated", item.DateCreated);
            dp.Add("@timeCreated", item.TimeCreated);
            dp.Add("@delivered", item.Delivered);
            dp.Add("@descriptions", item.Descriptions);
            var AffectedCount = await DataAccess.SaveDataAsync(InsertInvoiceItemQuery, dp);
            if (AffectedCount > 0)
            {
                item.Id = dp.Get<int>("@id");
                await UpdateItemUpdateDateAndUpdateTimeAsync(item.InvoiceId);
            }
            return AffectedCount;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
        return 0;
    }

    public async Task<int> UpdateInvoiceItemInDatabaseAsync(InvoiceItemModel item)
    {
        try
        {
            if (item == null || !item.IsCountStringValid) return 0;
            item.DateUpdated = PersianCalendarHelper.GetCurrentPersianDate();
            item.TimeUpdated = PersianCalendarHelper.GetCurrentTime();
            DynamicParameters dp = new();
            dp.Add("@id", item.Id);
            dp.Add("@productId", item.Product.Id);
            dp.Add("@buyPrice", item.BuyPrice);
            dp.Add("@sellPrice", item.SellPrice);
            dp.Add("@countString", item.CountString);
            dp.Add("@countValue", decimal.Round(item.CountValue, 3));
            dp.Add("@productUnitId", item.Unit == null ? (int?)null : item.Unit.Id);
            dp.Add("@dateUpdated", item.DateUpdated);
            dp.Add("@timeUpdated", item.TimeUpdated);
            dp.Add("@delivered", item.Delivered);
            dp.Add("@descriptions", item.Descriptions);
            var AffectedCount = await DataAccess.SaveDataAsync(UpdateInvoiceItemQuery, dp);
            if (AffectedCount > 0) await UpdateItemUpdateDateAndUpdateTimeAsync(item.InvoiceId);
            return AffectedCount;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
        return 0;
    }

    public async Task<int> DeleteInvoiceItemFromDatabaseAsync(int ItemId)
    {
        try
        {
            var InvoiceId = await GetInvoiceIdFromInvoiceItemId(ItemId);
            if (InvoiceId == 0) return 0;
            var dp = new DynamicParameters();
            dp.Add("@id", ItemId);
            var AffectedCount = await DataAccess.SaveDataAsync(DeleteInvoiceItemQuery, dp);
            if (AffectedCount > 0) await UpdateItemUpdateDateAndUpdateTimeAsync(InvoiceId);
            return AffectedCount;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
        return 0;
    }

    public async Task<InvoicePaymentModel> GetInvoicePaymentFromDatabaseAsync(int Id)
    {
        try
        {
            string GetInvoicePaymentQuery = $"SELECT * FROM InvoicePayments WHERE Id = { Id }";
            var result = await DataAccess.LoadDataAsync<InvoicePaymentModel>(GetInvoicePaymentQuery);
            return result.SingleOrDefault();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
        return null;
    }

    public async Task<int> InsertInvoicePaymentToDatabaseAsync(InvoicePaymentModel item)
    {
        try
        {
            if (item == null) return 0;
            item.DateCreated = PersianCalendarHelper.GetCurrentPersianDate();
            item.TimeCreated = PersianCalendarHelper.GetCurrentTime();
            DynamicParameters dp = new();
            dp.Add("@id", 0, DbType.Int32, ParameterDirection.Output);
            dp.Add("@invoiceId", item.InvoiceId);
            dp.Add("@dateCreated", item.DateCreated);
            dp.Add("@timeCreated", item.TimeCreated);
            dp.Add("@payAmount", item.PayAmount);
            dp.Add("@descriptions", item.Descriptions);
            var AffectedCount = await DataAccess.SaveDataAsync(InsertInvoicePaymentQuery, dp);
            if (AffectedCount > 0)
            {
                item.Id = dp.Get<int>("@id");
                await UpdateItemUpdateDateAndUpdateTimeAsync(item.InvoiceId);
            }
            return AffectedCount;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
        return 0;
    }

    public async Task<int> UpdateInvoicePaymentInDatabaseAsync(InvoicePaymentModel item)
    {
        try
        {
            if (item == null) return 0;
            item.DateUpdated = PersianCalendarHelper.GetCurrentPersianDate();
            item.TimeUpdated = PersianCalendarHelper.GetCurrentTime();
            DynamicParameters dp = new();
            dp.Add("@dateUpdated", item.DateUpdated);
            dp.Add("@timeUpdated", item.TimeUpdated);
            dp.Add("@payAmount", item.PayAmount);
            dp.Add("@descriptions", item.Descriptions);
            var AffectedCount = await DataAccess.SaveDataAsync(UpdateInvoicePaymentQuery, item);
            if (AffectedCount > 0) await UpdateItemUpdateDateAndUpdateTimeAsync(item.InvoiceId);
            return AffectedCount;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
        return 0;
    }

    public async Task<int> DeleteInvoicePaymentFromDatabaseAsync(int PaymentId)
    {
        try
        {
            var InvoiceId = await GetInvoiceIdFromInvoicePaymentId(PaymentId);
            if (InvoiceId == 0) return 0;
            DynamicParameters dp = new();
            dp.Add("@id", PaymentId);
            var AffectedCount = await DataAccess.SaveDataAsync(DeleteInvoicePaymentQuery, dp);
            if (AffectedCount > 0) await UpdateItemUpdateDateAndUpdateTimeAsync(InvoiceId);
            return AffectedCount;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
        return 0;
    }

    private async Task UpdateItemUpdateDateAndUpdateTimeAsync(int Id)
    {
        try
        {
            var dp = new DynamicParameters();
            dp.Add("@id", Id);
            dp.Add("@dateUpdated", PersianCalendarHelper.GetCurrentPersianDate());
            dp.Add("@timeUpdated", PersianCalendarHelper.GetCurrentTime());
            await DataAccess.SaveDataAsync(UpdateSubItemDateAndTimeQuery, dp).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
    }

    public async Task<int> GetTotalQueryCountAsync(string WhereClause)
    {
        try
        {
            var sqlTemp = $@"SELECT COUNT(i.[Id]) FROM Invoices i LEFT JOIN Customers c ON i.CustomerId = c.Id
                LEFT JOIN (SELECT SUM(ii.[CountValue] * ii.SellPrice) AS TotalSellValue, ii.[InvoiceId]
	            FROM InvoiceItems ii GROUP BY ii.[InvoiceId]) sp ON i.Id=sp.InvoiceId
                LEFT JOIN (SELECT SUM(ips.[PayAmount]) AS TotalPayments, ips.[InvoiceId]
	            FROM InvoicePayments ips GROUP BY ips.[InvoiceId]) pays ON i.Id=pays.InvoiceId
                { (string.IsNullOrEmpty(WhereClause) ? "" : " WHERE ") } { WhereClause }";
            return await DataAccess.ExecuteScalarAsync<int>(sqlTemp);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
        return 0;
    }

    public async Task<IEnumerable<InvoiceListModel>> LoadManyItemsAsync(int FetcheSize, int InvoiceId, int CustomerId, string InvoiceDate, string SearchValue, InvoiceLifeStatus? LifeStatus, InvoiceFinancialStatus? FinStatus, SqlQuerySearchMode SearchMode, OrderType orderType, int StartId)
    {
        try
        {
            DynamicParameters dp = new();
            dp.Add("@FetchSize", FetcheSize);
            dp.Add("@InvoiceId", InvoiceId);
            dp.Add("@CustomerId", CustomerId);
            dp.Add("@Date", InvoiceDate);
            dp.Add("@SearchValue", SearchValue);
            dp.Add("@LifeStatus", LifeStatus == null ? -1 : LifeStatus);
            dp.Add("@FinStatus", FinStatus == null ? -1 : FinStatus);
            dp.Add("@SearchMode", SearchMode);
            dp.Add("@OrderType", orderType);
            dp.Add("@StartId", StartId);
            var result = await DataAccess.LoadDataAsync<InvoiceListModel, DynamicParameters>("LoadInvoiceList_OldStyle", dp, CommandType.StoredProcedure);
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
        return null;
    }

    public async Task<InvoiceModel> LoadSingleItemAsync(int Id)
    {
        try
        {
            DynamicParameters dp = new();
            dp.Add("@InvoiceId", Id);
            using IDbConnection conn = new SqlConnection(DataAccess.GetConnectionString());
            var outPut = await conn.QueryMultipleAsync("LoadSingleInvoiceDetails", dp, commandType: CommandType.StoredProcedure);
            return outPut.MapToSingleInvoice();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
        return null;
    }

    public async Task<decimal> GetTotalOrRestTotalBalanceOfCustomerAsync(int CustomerId, int InvoiceId = 0)
    {
        try
        {
            var InvoiceClause = InvoiceId == 0 ? "" : $"AND i.[Id] <> { InvoiceId }";
            var sqlQuery = @$"SET NOCOUNT ON
                              SELECT SUM(dbo.CalculateDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue) - ISNULL(pays.TotalPayments, 0))
                              FROM Invoices i LEFT JOIN Customers c ON i.CustomerId = c.Id
                              
                              LEFT JOIN (SELECT SUM(ii.[CountValue] * ii.[SellPrice]) AS TotalSellValue, ii.[InvoiceId]
                                  FROM InvoiceItems ii GROUP BY ii.[InvoiceId]) sp ON i.Id=sp.InvoiceId
                              
                              LEFT JOIN (SELECT SUM(ips.[PayAmount]) AS TotalPayments, ips.[InvoiceId]
                                 FROM InvoicePayments ips GROUP BY ips.[InvoiceId]) pays ON i.Id=pays.InvoiceId
                              WHERE i.LifeStatus = { (int)InvoiceLifeStatus.Active } AND c.Id = { CustomerId } { InvoiceClause }
                              GROUP BY c.Id";
            return await DataAccess.ExecuteScalarAsync<decimal>(sqlQuery);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
        return 0;
    }

    public async Task<decimal> GetPrevBalanceOfInvoiceAsync(int InvoiceId)
    {
        try
        {
            string sqlQuery = $"SELECT dbo.CalculatePrevInvoiceAmount({InvoiceId});";
            return await DataAccess.ExecuteScalarAsync<decimal>(sqlQuery);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
        return 0;
    }

    public async Task<IEnumerable<ItemsForComboBox>> GetProductItemsAsync(string SearchText = null)
    {
        try
        {
            var where = string.IsNullOrEmpty(SearchText) ? "" : $" WHERE [ProductName] LIKE '%{ SearchText }%'";
            if (string.IsNullOrEmpty(where)) where = " WHERE IsActive = 1"; else where += " AND IsActive = 1";
            var sql = string.Format(GetProductItemsQuery, where);
            var items = await DataAccess.LoadDataAsync<ItemsForComboBox>(sql);
            return items;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
        return null;
    }

    public async Task<int> SetPrevInvoiceId(int InvoiceId, int PrevInvoiceId)
    {
        string sql = $"UPDATE Invoices SET [PrevInvoiceId] = { (PrevInvoiceId <= 0 ? "NULL" : PrevInvoiceId) } WHERE Id = {InvoiceId}";
        return await DataAccess.SaveDataAsync(sql);
    }

    public async Task<List<InvoiceListModel>> GetPrevInvoices(int InvoiceId, int CustomerId = -1, string InvoiceDate = null, string SearchValue = null, OrderType orderType = OrderType.DESC)
    {
        try
        {
            DynamicParameters dp = new();
            dp.Add("@InvoiceId", InvoiceId);
            dp.Add("@CustomerId", CustomerId);
            dp.Add("@Date", InvoiceDate);
            dp.Add("@SearchValue", SearchValue);
            dp.Add("@OrderType", orderType);
            var result = await DataAccess.LoadDataAsync<InvoiceListModel, DynamicParameters>("LoadPrevInvoiceList_OldStyle", dp, CommandType.StoredProcedure);
            return result.ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
        return null;
    }

    public async Task<IEnumerable<ProductUnitModel>> GetProductUnitsAsync()
    {
        try
        {
            var result = await DataAccess.LoadDataAsync<ProductUnitModel>(GetProductUnitsQuery);
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
        return null;
    }

    public async Task<IEnumerable<ItemsForComboBox>> GetCustomerNamesAsync(string SearchText)
    {
        try
        {
            var where = string.IsNullOrEmpty(SearchText) ? "" : $" WHERE [FirstName] + ' ' + [LastName] LIKE '%{ SearchText }%'";
            var sql = string.Format(GetCustomerNamesQuery, where);
            var items = await DataAccess.LoadDataAsync<ItemsForComboBox>(sql);
            return items;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
        return null;
    }

    public async Task<IEnumerable<ItemsForComboBox>> GetInvoiceAboutsAsync(string SearchText)
    {
        try
        {
            var where = string.IsNullOrWhiteSpace(SearchText) ? "" : $" AND TRIM([About]) LIKE '%{ SearchText }%'";
            var sql = string.Format(GetInvoiceAboutsQuery, where);
            var items = await DataAccess.LoadDataAsync<ItemsForComboBox>(sql);
            return items;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
        return null;
    }

    public async Task<IEnumerable<RecentSellPriceModel>> GetRecentSellPricesAsync(int MaxRecord, int CustomerId, int ProductId)
    {
        try
        {
            var sql = string.Format(GetRecentPricesOfProductQuery, MaxRecord, CustomerId, ProductId);
            return await DataAccess.LoadDataAsync<RecentSellPriceModel>(sql);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
        return null;
    }

    public ValidationResult ValidateItem(InvoiceModel item)
    {
        try
        {
            InvoiceValidator validator = new();
            var result = validator.Validate(item);
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
        return null;
    }

    public ValidationResult ValidateItem(InvoiceItemModel item)
    {
        try
        {
            InvoiceItemValidator validator = new();
            var result = validator.Validate(item);
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
        return null;
    }

    public ValidationResult ValidateItem(InvoicePaymentModel item)
    {
        try
        {
            InvoicePaymentValidator validator = new();
            var result = validator.Validate(item);
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
        return null;
    }
}

internal class CustomerModelMapper : EntityMap<CustomerModel>
{
    public CustomerModelMapper()
    {
        try
        {
            Map(x => x.Id).ToColumn("CustId");
            Map(x => x.Descriptions).ToColumn("CustDescriptions");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
    }
}
internal class ProductModelMapper : EntityMap<ProductModel>
{
    public ProductModelMapper()
    {
        try
        {
            Map(x => x.Id).ToColumn("pId");
            Map(x => x.BuyPrice).ToColumn("pBuyPrice");
            Map(x => x.SellPrice).ToColumn("pSellPrice");
            Map(x => x.CountString).ToColumn("pCountString");
            Map(x => x.DateCreated).ToColumn("pDateCreated");
            Map(x => x.TimeCreated).ToColumn("pTimeCreated");
            Map(x => x.DateUpdated).ToColumn("pDateUpdated");
            Map(x => x.TimeUpdated).ToColumn("pTimeUpdated");
            Map(x => x.Descriptions).ToColumn("pDescriptions");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
    }
}
internal class ProductUnitModelMapper : EntityMap<ProductUnitModel>
{
    public ProductUnitModelMapper()
    {
        try
        {
            Map(x => x.Id).ToColumn("puId");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
    }
}