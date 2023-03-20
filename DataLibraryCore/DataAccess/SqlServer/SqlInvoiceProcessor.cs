using Dapper;
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
    private const string QueryOrderBy = "Id";
    private const OrderType QueryOrderType = OrderType.DESC;
    private readonly string CreateInvoiceQuery = @"DECLARE @newId int; SET @newId = (SELECT ISNULL(MAX([Id]), 1) FROM [Invoices]) + 1;
            INSERT INTO Invoices ([Id], CustomerId, DateCreated, DiscountType, DiscountValue, Descriptions, LifeStatus)
            VALUES (@newId, @customerId, @dateCreated, @discountType, @discountValue, @descriptions, @lifeStatus);
            SELECT @id = @newId;";
    private readonly string UpdateInvoiceQuery = @"UPDATE Invoices SET CustomerId = @customerId, DateCreated = @dateCreated,
            DateUpdated = @dateUpdated, DiscountType = @discountType,
            DiscountValue = @discountValue, Descriptions = @descriptions, LifeStatus = @lifeStatus WHERE Id = @id";
    private readonly string InsertInvoiceItemQuery = @"DECLARE @newId int; SET @newId = (SELECT ISNULL(MAX([Id]), 0) FROM [InvoiceItems]) + 1;
            INSERT INTO InvoiceItems ([Id], InvoiceId, ProductId, BuyPrice, SellPrice, CountString, CountValue, ProductUnitId, DateCreated, Delivered, Descriptions)
            VALUES (@newId, @invoiceId, @productId, @buyPrice, @sellPrice, @countString, @countValue, @productUnitId, @dateCreated, @delivered, @descriptions);
            SELECT @id = @newId;";
    private readonly string UpdateInvoiceItemQuery = @$"UPDATE InvoiceItems SET ProductId = @productId, BuyPrice = @buyPrice, SellPrice = @sellPrice,
            CountString = @countString, CountValue = @countValue, ProductUnitId = @productUnitId, DateUpdated = @dateUpdated, Delivered = @delivered, Descriptions = @descriptions WHERE [Id] = @id";
    private readonly string UpdateSubItemDateAndTimeQuery = @"UPDATE Invoices SET DateUpdated = @dateUpdated, TimeUpdated = @timeUpdated WHERE [Id] = @id";
    private readonly string DeleteInvoiceItemQuery = @$"DELETE FROM InvoiceItems WHERE [Id] = @id";
    private readonly string InsertInvoicePaymentQuery = @$"DECLARE @newId int; SET @newId = (SELECT ISNULL(MAX([Id]), 0) FROM [InvoicePayments]) + 1;
            INSERT INTO InvoicePayments ([Id], InvoiceId, DateCreated, PayAmount, Descriptions)
            VALUES (@newId, @invoiceId, @dateCreated, @payAmount, @descriptions);
            SELECT @id = @newId;";
    private readonly string UpdateInvoicePaymentQuery = @$"UPDATE InvoicePayments SET DateUpdated = @dateUpdated,
            PayAmount = @payAmount, Descriptions = @descriptions WHERE [Id] = @id";
    private readonly string DeleteInvoicePaymentQuery = @$"DELETE FROM InvoicePayments WHERE [Id] = @id";
    private readonly string LoadSingleItemQuery = @"SET NOCOUNT ON
            DECLARE @invoices TABLE(
	        [Id] [int],
            [CustomerId] [int],
	        [DateCreated] [datetime],
	        [DateUpdated] [datetime],
	        [DiscountType] [tinyint],
	        [DiscountValue] [float],
			[About] [nvarchar](50),
	        [Descriptions] [ntext],
	        [LifeStatus] [tinyint],
			[PrevInvoiceId] [int],

	        [CustId] [int],
	        [FirstName] [nvarchar](50),
	        [LastName] [nvarchar](50),
	        [CompanyName] [nvarchar](50),
	        [EmailAddress] [nvarchar](50),
	        [PostAddress] [ntext],
	        [DateJoined] [datetime],
	        [CustDescriptions] [ntext])

            INSERT @invoices
            SELECT i.*, c.Id as CustId, FirstName, LastName, CompanyName, EmailAddress, PostAddress, DateJoined, c.Descriptions as CustDescriptions
            FROM Invoices i LEFT JOIN Customers c ON i.CustomerId = c.Id
            WHERE i.Id = {0}

            SELECT * FROM @invoices ORDER BY [Id] ASC;
            SELECT it.Id, it.InvoiceId, it.BuyPrice, it.SellPrice, it.CountString, it.DateCreated,
                it.DateUpdated, it.Delivered, it.Descriptions, p.Id pId, p.ProductName, p.BuyPrice pBuyPrice,
                p.SellPrice pSellPrice, p.Barcode, p.CountString pCountString, p.DateCreated pDateCreated, p.DateUpdated pDateUpdated, p.Descriptions pDescriptions, u.Id AS puId, u.UnitName
                FROM InvoiceItems it LEFT JOIN Products p ON it.ProductId = p.Id LEFT JOIN ProductUnits u ON it.ProductUnitId = u.Id WHERE it.InvoiceId IN (SELECT i.Id FROM @invoices i) ORDER BY [Id] DESC;
            SELECT * FROM InvoicePayments WHERE InvoiceId IN (SELECT i.Id FROM @invoices i);
            SELECT * FROM PhoneNumbers WHERE CustomerId IN (SELECT i.CustomerId FROM @invoices i);";
    private readonly string GetSingleInvoiceItemQuery = @"SELECT it.*, p.[Id] AS pId,
                p.[ProductName], p.[BuyPrice] AS pBuyPrice, p.[SellPrice] AS pSellPrice, p.[Barcode],
                p.[CountString] AS pCountString, p.[DateCreated] AS pDateCreated, p.[DateUpdated] AS pDateUpdated, p.[Descriptions] AS pDescriptions, u.Id AS puId, u.UnitName
                FROM InvoiceItems it LEFT JOIN Products p ON it.ProductId = p.Id LEFT JOIN ProductUnits u ON it.ProductUnitId = u.Id WHERE it.Id = {0}";
    private readonly string GetProductItemsQuery = "SELECT [Id], [ProductName] AS ItemName FROM Products {0} ORDER BY [ProductName]";
    private readonly string GetProductUnitsQuery = "SELECT [Id], [UnitName] FROM ProductUnits";
    private readonly string GetCustomerNamesQuery = "SELECT [Id], ISNULL(FirstName, '') + ' ' + ISNULL(LastName, '') AS ItemName FROM Customers {0} ORDER BY [FirstName], [LastName]";
    private readonly string GetRecentPricesOfProductQuery = @"SELECT TOP({0}) it.SellPrice AS SellPrice, CONVERT(VARCHAR(10), it.DateCreated, 111) AS DateSold FROM InvoiceItems it LEFT JOIN Invoices i ON it.InvoiceId = i.Id
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
                      {mode} i.[DateUpdated] LIKE { criteria }
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
                      {mode} CAST(ISNULL(dbo.GetDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue), 0) - ISNULL(pays.[TotalPayments], 0) AS varchar) LIKE { criteria }
                      {mode} CAST(ISNULL(dbo.GetDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue), 0) AS varchar) LIKE { criteria } )
                      {(LifeStatus == null ? "" : $" AND i.[LifeStatus] = { (int)LifeStatus } ")}
                      {(FinStatus == null ? "" : $" AND ISNULL(dbo.GetDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue), 0) - ISNULL(pays.TotalPayments, 0) { finStatusOperand } 0 ")}";
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
            item.DateCreated = DateTime.Now;
            var dp = new DynamicParameters();
            dp.Add("@id", 0, DbType.Int32, ParameterDirection.Output);
            dp.Add("@customerId", item.Customer.Id);
            dp.Add("@dateCreated", item.DateCreated);
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
            dp.Add("@dateCreated", item.DateCreated);
            dp.Add("@dateUpdated", DateTime.Now);
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
            item.DateCreated = DateTime.Now;
            DynamicParameters dp = new();
            dp.Add("@id", 0, DbType.Int32, ParameterDirection.Output);
            dp.Add("@invoiceId", item.InvoiceId);
            dp.Add("@productId", item.Product.Id);
            dp.Add("@buyPrice", item.BuyPrice);
            dp.Add("@sellPrice", item.SellPrice);
            dp.Add("@countString", item.CountString);
            dp.Add("@countValue", item.CountValue);
            dp.Add("@productUnitId", item.Unit == null ? (int?)null : item.Unit.Id);
            dp.Add("@dateCreated", item.DateCreated);
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
            item.DateUpdated = DateTime.Now;
            DynamicParameters dp = new();
            dp.Add("@id", item.Id);
            dp.Add("@productId", item.Product.Id);
            dp.Add("@buyPrice", item.BuyPrice);
            dp.Add("@sellPrice", item.SellPrice);
            dp.Add("@countString", item.CountString);
            dp.Add("@countValue", item.CountValue);
            dp.Add("@productUnitId", item.Unit == null ? (int?)null : item.Unit.Id);
            dp.Add("@dateUpdated", item.DateUpdated);
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
            item.DateCreated = DateTime.Now;
            DynamicParameters dp = new();
            dp.Add("@id", 0, DbType.Int32, ParameterDirection.Output);
            dp.Add("@invoiceId", item.InvoiceId);
            dp.Add("@dateCreated", item.DateCreated);
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
            item.DateUpdated = DateTime.Now;
            DynamicParameters dp = new();
            dp.Add("@dateUpdated", item.DateUpdated);
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
            dp.Add("@dateUpdated", DateTime.Now);
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

    public async Task<IEnumerable<InvoiceListModel>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, string OrderBy = QueryOrderBy, OrderType Order = QueryOrderType)
    {
        try
        {
            string sql = $@"SET NOCOUNT ON
                            SELECT i.Id, i.CustomerId, ISNULL(c.FirstName, '') + ' ' + ISNULL(c.LastName, '') CustomerFullName, i.DateCreated, i.DateUpdated,
		                            dbo.GetDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue) AS TotalInvoiceSum, pays.TotalPayments, i.LifeStatus
                            FROM Invoices i LEFT JOIN Customers c ON i.CustomerId = c.Id
                            
                            LEFT JOIN (SELECT SUM(ii.[CountValue] * ii.SellPrice) AS TotalSellValue, ii.[InvoiceId]
	                            FROM InvoiceItems ii GROUP BY ii.[InvoiceId]) sp ON i.Id = sp.InvoiceId
                            
                            LEFT JOIN (SELECT SUM(ips.[PayAmount]) AS TotalPayments, ips.[InvoiceId]
	                           FROM InvoicePayments ips GROUP BY ips.[InvoiceId]) pays ON i.Id = pays.InvoiceId

                            { (string.IsNullOrEmpty(WhereClause) ? "" : $" WHERE { WhereClause }") }
                            ORDER BY [{OrderBy}] {Order} OFFSET {OffSet} ROWS FETCH NEXT {FetcheSize} ROWS ONLY";
            return await DataAccess.LoadDataAsync<InvoiceListModel>(sql);
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
            var query = string.Format(LoadSingleItemQuery, Id);
            using IDbConnection conn = new SqlConnection(DataAccess.GetConnectionString());
            var outPut = await conn.QueryMultipleAsync(query);
            return outPut.MapToSingleInvoice();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlInvoiceProcessor");
        }
        return null;
    }

    public async Task<double> GetTotalOrRestTotalBalanceOfCustomerAsync(int CustomerId, int InvoiceId = 0)
    {
        try
        {
            var InvoiceClause = InvoiceId == 0 ? "" : $"AND i.[Id] <> { InvoiceId }";
            var sqlQuery = @$"SET NOCOUNT ON
                              SELECT SUM(dbo.GetDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue) - ISNULL(pays.TotalPayments, 0))
                              FROM Invoices i LEFT JOIN Customers c ON i.CustomerId = c.Id
                              
                              LEFT JOIN (SELECT SUM(ii.[CountValue] * ii.[SellPrice]) AS TotalSellValue, ii.[InvoiceId]
                                  FROM InvoiceItems ii GROUP BY ii.[InvoiceId]) sp ON i.Id=sp.InvoiceId
                              
                              LEFT JOIN (SELECT SUM(ips.[PayAmount]) AS TotalPayments, ips.[InvoiceId]
                                 FROM InvoicePayments ips GROUP BY ips.[InvoiceId]) pays ON i.Id=pays.InvoiceId
                              WHERE i.LifeStatus = { (int)InvoiceLifeStatus.Active } AND c.Id = { CustomerId } { InvoiceClause }
                              GROUP BY c.Id";
            return await DataAccess.ExecuteScalarAsync<double>(sqlQuery);
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
            Map(x => x.DateUpdated).ToColumn("pDateUpdated");
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