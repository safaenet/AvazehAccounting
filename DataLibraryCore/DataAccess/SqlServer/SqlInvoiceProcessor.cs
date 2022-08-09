using Dapper;
using DataLibraryCore.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.ObjectModel;
using FluentValidation.Results;
using Dapper.FluentMap.Mapping;
using Dapper.FluentMap;
using DataLibraryCore.DataAccess.Interfaces;
using SharedLibrary.Enums;
using SharedLibrary.DalModels;
using SharedLibrary.Validators;
using SharedLibrary.DtoModels;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.SqlServer
{
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
                catch { }
                
            }
        }

        private readonly IDataAccess DataAccess;
        private const string QueryOrderBy = "Id";
        private const OrderType QueryOrderType = OrderType.DESC;
        private readonly string CreateInvoiceQuery = @"INSERT INTO Invoices (CustomerId, DateCreated, TimeCreated, DiscountType, DiscountValue, Descriptions, LifeStatus)
            VALUES (@customerId, @dateCreated, @timeCreated, @discountType, @discountValue, @descriptions, @lifeStatus); SELECT @id = @@IDENTITY;";
        private readonly string UpdateInvoiceQuery = @"UPDATE Invoices SET CustomerId = @customerId, DateCreated = @dateCreated, TimeCreated = @timeCreated,
            DateUpdated = @dateUpdated, TimeUpdated = @timeUpdated, DiscountType = @discountType,
            DiscountValue = @discountValue, Descriptions = @descriptions, LifeStatus = @lifeStatus WHERE Id = @id";
        private readonly string InsertInvoiceItemQuery = @"INSERT INTO InvoiceItems (InvoiceId, ProductId, BuyPrice, SellPrice, CountString, CountValue, ProductUnitId, DateCreated,
            TimeCreated, Delivered, Descriptions)
            VALUES (@invoiceId, @productId, @buyPrice, @sellPrice, @countString, @countValue, @productUnitId, @dateCreated, @timeCreated, @delivered, @descriptions);
            SELECT @id = @@IDENTITY;";
        private readonly string UpdateInvoiceItemQuery = @$"UPDATE InvoiceItems SET ProductId = @productId, BuyPrice = @buyPrice, SellPrice = @sellPrice,
            CountString = @countString, CountValue = @countValue, ProductUnitId = @productUnitId, DateUpdated = @dateUpdated, TimeUpdated = @timeUpdated,
            Delivered = @delivered, Descriptions = @descriptions WHERE [Id] = @id";
        private readonly string UpdateSubItemDateAndTimeQuery = @"UPDATE Invoices SET DateUpdated = @dateUpdated, TimeUpdated = @timeUpdated WHERE [Id] = @id";
        private readonly string DeleteInvoiceItemQuery = @$"DELETE FROM InvoiceItems WHERE [Id] = @id";
        private readonly string InsertInvoicePaymentQuery = @$"INSERT INTO InvoicePayments (InvoiceId, DateCreated, TimeCreated, PayAmount, Descriptions)
            VALUES (@invoiceId, @dateCreated, @timeCreated, @payAmount, @descriptions); SELECT @id = @@IDENTITY;";
        private readonly string UpdateInvoicePaymentQuery = @$"UPDATE InvoicePayments SET DateUpdated = @dateUpdated, TimeUpdated = @timeUpdated,
            PayAmount = @payAmount, Descriptions = @descriptions WHERE [Id] = @id";
        private readonly string DeleteInvoicePaymentQuery = @$"DELETE FROM InvoicePayments WHERE [Id] = @id";
        private readonly string LoadSingleItemQuery = @"SET NOCOUNT ON
            DECLARE @invoices TABLE(
	        [Id] [int],
            [CustomerId] [int],
	        [DateCreated] [char](10),
	        [TimeCreated] [char](8),
	        [DateUpdated] [char](10),
	        [TimeUpdated] [char](8),
	        [DiscountType] [tinyint],
	        [DiscountValue] [float],
	        [Descriptions] [ntext],
	        [LifeStatus] [tinyint],

	        [CustId] [int],
	        [FirstName] [nvarchar](50),
	        [LastName] [nvarchar](50),
	        [CompanyName] [nvarchar](50),
	        [EmailAddress] [nvarchar](50),
	        [PostAddress] [ntext],
	        [DateJoined] [char](10),
	        [CustDescriptions] [ntext])

            INSERT @invoices
            SELECT i.*, c.Id as CustId, FirstName, LastName, CompanyName, EmailAddress, PostAddress, DateJoined, c.Descriptions as CustDescriptions
            FROM Invoices i LEFT JOIN Customers c ON i.CustomerId = c.Id
            WHERE i.Id = {0}

            SELECT * FROM @invoices ORDER BY [Id] ASC;
            SELECT it.Id, it.InvoiceId, it.BuyPrice, it.SellPrice, it.CountString, it.DateCreated, it.TimeCreated,
                it.DateUpdated, it.TimeUpdated, it.Delivered, it.Descriptions, p.Id pId, p.ProductName, p.BuyPrice pBuyPrice,
                p.SellPrice pSellPrice, p.Barcode, p.CountString pCountString, p.DateCreated pDateCreated,
                p.TimeCreated pTimeCreated, p.DateUpdated pDateUpdated, p.TimeUpdated pTimeUpdated, p.Descriptions pDescriptions,
                u.Id AS puId, u.UnitName
                FROM InvoiceItems it LEFT JOIN Products p ON it.ProductId = p.Id LEFT JOIN ProductUnits u ON it.ProductUnitId = u.Id WHERE it.InvoiceId IN (SELECT i.Id FROM @invoices i);
            SELECT * FROM InvoicePayments WHERE InvoiceId IN (SELECT i.Id FROM @invoices i);
            SELECT * FROM PhoneNumbers WHERE CustomerId IN (SELECT i.CustomerId FROM @invoices i);";
        private readonly string GetSingleInvoiceItemQuery = @"SELECT it.*, p.[Id] AS pId,
                p.[ProductName], p.[BuyPrice] AS pBuyPrice, p.[SellPrice] AS pSellPrice, p.[Barcode],
                p.[CountString] AS pCountString, p.[DateCreated] AS pDateCreated, p.[TimeCreated] AS pTimeCreated,
                p.[DateUpdated] AS pDateUpdated, p.[TimeUpdated] AS pTimeUpdated, p.[Descriptions] AS pDescriptions,
				u.Id AS puId, u.UnitName
                FROM InvoiceItems it LEFT JOIN Products p ON it.ProductId = p.Id LEFT JOIN ProductUnits u ON it.ProductUnitId = u.Id WHERE it.Id = {0}";
        private readonly string GetProductItemsQuery = "SELECT [Id], [ProductName] AS ItemName FROM Products {0}";
        private readonly string GetProductUnitsQuery = "SELECT [Id], [UnitName] FROM ProductUnits";
        private readonly string GetCustomerNamesQuery = "SELECT [Id], [FirstName] + ' ' + [LastName] AS ItemName FROM Customers {0} ORDER BY [FirstName], [LastName]";
        private readonly string GetRecentPricesOfProduct = @"SELECT TOP({0}) it.SellPrice AS SellPrice, it.DateCreated AS DateSold FROM InvoiceItems it LEFT JOIN Invoices i ON it.InvoiceId = i.Id
                                                             LEFT JOIN Customers c ON i.CustomerId = c.Id LEFT JOIN Products p ON it.ProductId = p.Id
                                                             WHERE c.Id = {1} AND p.Id = {2} ORDER BY it.DateCreated DESC";

        private async Task<int> GetInvoiceIdFromInvoiceItemId(int Id)
        {
            var query = $"SELECT InvoiceId FROM InvoiceItems WHERE Id = { Id }";
            return await DataAccess.ExecuteScalarAsync<int, DynamicParameters>(query, null);
        }

        private async Task<int> GetInvoiceIdFromInvoicePaymentId(int Id)
        {
            var query = $"SELECT InvoiceId FROM InvoicePayments WHERE Id = { Id }";
            return await DataAccess.ExecuteScalarAsync<int, DynamicParameters>(query, null);
        }

        public string GenerateWhereClause(string val, InvoiceLifeStatus? LifeStatus, InvoiceFinancialStatus? FinStatus, SqlSearchMode mode = SqlSearchMode.OR)
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
                      {mode} i.[Descriptions] LIKE { criteria }

                      {mode} CAST(c.[Id] AS VARCHAR) LIKE { criteria }
                      {mode} c.[FirstName] + ' ' + c.[LastName] LIKE { criteria }
                      {mode} c.[CompanyName] LIKE { criteria }
                      {mode} c.[EmailAddress] LIKE { criteria }
                      {mode} c.[PostAddress] LIKE { criteria }
                      {mode} c.[DateJoined] LIKE { criteria }
                      {mode} c.[Descriptions] LIKE { criteria }

                      {mode} CAST(sp.[TotalSellValue] AS varchar) LIKE { criteria }
                      {mode} CAST(pays.[TotalPayments] AS varchar) LIKE { criteria }
                      {mode} CAST(ISNULL(dbo.GetDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue), 0) - ISNULL(pays.[TotalPayments], 0) AS varchar) LIKE { criteria }
                      {mode} CAST(ISNULL(dbo.GetDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue), 0) AS varchar) LIKE { criteria } )
                      {(LifeStatus == null ? "" : $" AND i.[LifeStatus] = { (int)LifeStatus } ")}
                      {(FinStatus == null ? "" : $" AND ISNULL(dbo.GetDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue), 0) - ISNULL(pays.TotalPayments, 0) { finStatusOperand } 0 ")}";
        }

        public async Task<int> CreateItemAsync(InvoiceModel item)
        {
            if (item == null || !ValidateItem(item).IsValid) return 0;
            item.DateCreated = PersianCalendarModel.GetCurrentPersianDate();
            item.TimeCreated = PersianCalendarModel.GetCurrentTime();
            var dp = new DynamicParameters();
            dp.Add("@id", 0, DbType.Int32, ParameterDirection.Output);
            dp.Add("@customerId", item.Customer.Id);
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

        public async Task<int> UpdateItemAsync(InvoiceModel item)
        {
            if (item == null || !ValidateItem(item).IsValid) return 0;
            var dp = new DynamicParameters();
            dp.Add("@id", item.Id);
            dp.Add("@customerId", item.Customer.Id);
            dp.Add("@dateCreated", item.DateCreated);
            dp.Add("@timeCreated", item.TimeCreated);
            dp.Add("@dateUpdated", PersianCalendarModel.GetCurrentPersianDate());
            dp.Add("@timeUpdated", PersianCalendarModel.GetCurrentTime());
            dp.Add("@discountType", item.DiscountType);
            dp.Add("@discountValue", item.DiscountValue);
            dp.Add("@descriptions", item.Descriptions);
            dp.Add("@lifeStatus", item.LifeStatus);
            return await DataAccess.SaveDataAsync(UpdateInvoiceQuery, dp);
        }

        public async Task<int> DeleteItemByIdAsync(int Id)
        {
            string sql = @$"DELETE FROM Invoices WHERE Id = {Id}";
            return await DataAccess.SaveDataAsync<DynamicParameters>(sql, null);
        }

        public async Task<InvoiceItemModel> GetInvoiceItemFromDatabaseAsync(int Id)
        {
            var sql = string.Format(GetSingleInvoiceItemQuery, Id);
            using IDbConnection conn = new SqlConnection(DataAccess.GetConnectionString());
            var result = await conn.QueryAsync<InvoiceItemModel, ProductModel, ProductUnitModel, InvoiceItemModel>
                (sql, (it, p, u) => { it.Product = p; it.Unit = u; return it; }, splitOn: "pId, puId");
            return result.SingleOrDefault();
        }

        public async Task<int> InsertInvoiceItemToDatabaseAsync(InvoiceItemModel item)
        {
            if (item == null || !item.IsCountStringValid) return 0;
            item.DateCreated = PersianCalendarModel.GetCurrentPersianDate();
            item.TimeCreated = PersianCalendarModel.GetCurrentTime();
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

        public async Task<int> UpdateInvoiceItemInDatabaseAsync(InvoiceItemModel item)
        {
            if (item == null || !item.IsCountStringValid) return 0;
            item.DateUpdated = PersianCalendarModel.GetCurrentPersianDate();
            item.TimeUpdated = PersianCalendarModel.GetCurrentTime();
            DynamicParameters dp = new();
            dp.Add("@id", item.Id);
            dp.Add("@productId", item.Product.Id);
            dp.Add("@buyPrice", item.BuyPrice);
            dp.Add("@sellPrice", item.SellPrice);
            dp.Add("@countString", item.CountString);
            dp.Add("@countValue", item.CountValue);
            dp.Add("@productUnitId", item.Unit == null ? (int?)null : item.Unit.Id);
            dp.Add("@dateUpdated", item.DateUpdated);
            dp.Add("@timeUpdated", item.TimeUpdated);
            dp.Add("@delivered", item.Delivered);
            dp.Add("@descriptions", item.Descriptions);
            var AffectedCount = await DataAccess.SaveDataAsync(UpdateInvoiceItemQuery, dp);
            if (AffectedCount > 0) await UpdateItemUpdateDateAndUpdateTimeAsync(item.InvoiceId);
            return AffectedCount;
        }

        public async Task<int> DeleteInvoiceItemFromDatabaseAsync(int ItemId)
        {
            var InvoiceId = await GetInvoiceIdFromInvoiceItemId(ItemId);
            if (InvoiceId == 0) return 0;
            var dp = new DynamicParameters();
            dp.Add("@id", ItemId);
            var AffectedCount = await DataAccess.SaveDataAsync(DeleteInvoiceItemQuery, dp);
            if (AffectedCount > 0) await UpdateItemUpdateDateAndUpdateTimeAsync(InvoiceId);
            return AffectedCount;
        }

        public async Task<InvoicePaymentModel> GetInvoicePaymentFromDatabaseAsync(int Id)
        {
            string GetInvoicePaymentQuery = $"SELECT * FROM InvoicePayments WHERE Id = { Id }";
            var result = await DataAccess.LoadDataAsync<InvoicePaymentModel, DynamicParameters>(GetInvoicePaymentQuery, null);
            return result.SingleOrDefault();
        }

        public async Task<int> InsertInvoicePaymentToDatabaseAsync(InvoicePaymentModel item)
        {
            if (item == null) return 0;
            item.DateCreated = PersianCalendarModel.GetCurrentPersianDate();
            item.TimeCreated = PersianCalendarModel.GetCurrentTime();
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

        public async Task<int> UpdateInvoicePaymentInDatabaseAsync(InvoicePaymentModel item)
        {
            if (item == null) return 0;
            item.DateUpdated = PersianCalendarModel.GetCurrentPersianDate();
            item.TimeUpdated = PersianCalendarModel.GetCurrentTime();
            DynamicParameters dp = new();
            dp.Add("@dateUpdated", item.DateUpdated);
            dp.Add("@timeUpdated", item.TimeUpdated);
            dp.Add("@payAmount", item.PayAmount);
            dp.Add("@descriptions", item.Descriptions);
            var AffectedCount = await DataAccess.SaveDataAsync(UpdateInvoicePaymentQuery, item);
            if (AffectedCount > 0) await UpdateItemUpdateDateAndUpdateTimeAsync(item.InvoiceId);
            return AffectedCount;
        }

        public async Task<int> DeleteInvoicePaymentFromDatabaseAsync(int PaymentId)
        {
            var InvoiceId = await GetInvoiceIdFromInvoicePaymentId(PaymentId);
            if (InvoiceId == 0) return 0;
            DynamicParameters dp = new();
            dp.Add("@id", PaymentId);
            var AffectedCount = await DataAccess.SaveDataAsync(DeleteInvoicePaymentQuery, dp);
            if (AffectedCount > 0) await UpdateItemUpdateDateAndUpdateTimeAsync(InvoiceId);
            return AffectedCount;
        }

        private async Task UpdateItemUpdateDateAndUpdateTimeAsync(int Id)
        {
            var dp = new DynamicParameters();
            dp.Add("@id", Id);
            dp.Add("@dateUpdated", PersianCalendarModel.GetCurrentPersianDate());
            dp.Add("@timeUpdated", PersianCalendarModel.GetCurrentTime());
            await DataAccess.SaveDataAsync(UpdateSubItemDateAndTimeQuery, dp).ConfigureAwait(false);
        }

        public async Task<int> GetTotalQueryCountAsync(string WhereClause)
        {
            var sqlTemp = $@"SELECT COUNT(i.[Id]) FROM Invoices i LEFT JOIN Customers c ON i.CustomerId = c.Id
                LEFT JOIN (SELECT SUM(ii.[CountValue] * ii.SellPrice) AS TotalSellValue, ii.[InvoiceId]
	            FROM InvoiceItems ii GROUP BY ii.[InvoiceId]) sp ON i.Id=sp.InvoiceId
                LEFT JOIN (SELECT SUM(ips.[PayAmount]) AS TotalPayments, ips.[InvoiceId]
	            FROM InvoicePayments ips GROUP BY ips.[InvoiceId]) pays ON i.Id=pays.InvoiceId
                { (string.IsNullOrEmpty(WhereClause) ? "" : " WHERE ") } { WhereClause }";
            return await DataAccess.ExecuteScalarAsync<int, DynamicParameters>(sqlTemp, null);
        }

        public async Task<ObservableCollection<InvoiceListModel>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, string OrderBy = QueryOrderBy, OrderType Order = QueryOrderType)
        {
            string sql = $@"--CREATE FUNCTION dbo.GetDiscountedInvoiceSum(@disType tinyint, @disVal float, @amountVal float)
                            --RETURNS FLOAT
                            --AS
                            --BEGIN
                            --RETURN  CASE
                            --		WHEN @disType = 0 THEN @amountVal - (@disVal / 100 * @amountVal)
                            --		WHEN @disType = 1 THEN @amountVal - @disVal
                            --		END
                            --END

                            SET NOCOUNT ON
                            SELECT i.Id, i.CustomerId, c.FirstName + ' ' + c.LastName CustomerFullName, i.DateCreated, i.TimeCreated, i.DateUpdated, i.TimeUpdated,
		                            dbo.GetDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue) AS TotalInvoiceSum,
		                            pays.TotalPayments, i.LifeStatus
                            FROM Invoices i LEFT JOIN Customers c ON i.CustomerId = c.Id
                            
                            LEFT JOIN (SELECT SUM(ii.[CountValue] * ii.SellPrice) AS TotalSellValue, ii.[InvoiceId]
	                            FROM InvoiceItems ii GROUP BY ii.[InvoiceId]) sp ON i.Id = sp.InvoiceId
                            
                            LEFT JOIN (SELECT SUM(ips.[PayAmount]) AS TotalPayments, ips.[InvoiceId]
	                           FROM InvoicePayments ips GROUP BY ips.[InvoiceId]) pays ON i.Id = pays.InvoiceId

                            { (string.IsNullOrEmpty(WhereClause) ? "" : $" WHERE { WhereClause }") }
                            ORDER BY [{OrderBy}] {Order} OFFSET {OffSet} ROWS FETCH NEXT {FetcheSize} ROWS ONLY";
            return await DataAccess.LoadDataAsync<InvoiceListModel, DynamicParameters>(sql, null);
        }

        public async Task<InvoiceModel> LoadSingleItemAsync(int Id)
        {
            var query = string.Format(LoadSingleItemQuery, Id);
            using IDbConnection conn = new SqlConnection(DataAccess.GetConnectionString());
            var outPut = await conn.QueryMultipleAsync(query);
            return outPut.MapToSingleInvoice();
        }

        public async Task<double> GetTotalOrRestTotalBalanceOfCustomerAsync(int CustomerId, int InvoiceId = 0)
        {
            var InvoiceClause = InvoiceId == 0 ? "" : $"AND i.[Id] <> { InvoiceId }";
            var sqlQuery = @$"SET NOCOUNT ON
                              SELECT SUM(ISNULL(dbo.GetDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue), 0) - ISNULL(pays.TotalPayments, 0))
                              FROM Invoices i LEFT JOIN Customers c ON i.CustomerId = c.Id
                              
                              LEFT JOIN (SELECT SUM(ii.[CountValue] * ii.[SellPrice]) AS TotalSellValue, ii.[InvoiceId]
                                  FROM InvoiceItems ii GROUP BY ii.[InvoiceId]) sp ON i.Id=sp.InvoiceId
                              
                              LEFT JOIN (SELECT SUM(ips.[PayAmount]) AS TotalPayments, ips.[InvoiceId]
                                 FROM InvoicePayments ips GROUP BY ips.[InvoiceId]) pays ON i.Id=pays.InvoiceId
                              WHERE i.LifeStatus = { (int)InvoiceLifeStatus.Active } AND c.Id = { CustomerId } { InvoiceClause }
                              GROUP BY c.Id";
            return await DataAccess.ExecuteScalarAsync<double, DynamicParameters>(sqlQuery, null);
        }

        public async Task<List<ItemsForComboBox>> GetProductItemsAsync(string SearchText = null)
        {
            var where = string.IsNullOrEmpty(SearchText) ? "" : $" WHERE [ProductName] LIKE '%{ SearchText }%'";
            var sql = string.Format(GetProductItemsQuery, where);
            var items = await DataAccess.LoadDataAsync<ItemsForComboBox, DynamicParameters>(sql, null);
            return items?.ToList();
        }

        public async Task<List<ProductUnitModel>> GetProductUnitsAsync()
        {
            var result = await DataAccess.LoadDataAsync<ProductUnitModel, DynamicParameters>(GetProductUnitsQuery, null);
            return result?.ToList();
        }

        public async Task<List<ItemsForComboBox>> GetCustomerNamesAsync(string SearchText)
        {
            var where = string.IsNullOrEmpty(SearchText) ? "" : $" WHERE [FirstName] + ' ' + [LastName] LIKE '%{ SearchText }%'";
            var sql = string.Format(GetCustomerNamesQuery, where);
            var items = await DataAccess.LoadDataAsync<ItemsForComboBox, DynamicParameters>(sql, null);
            return items?.ToList();
        }

        public async Task<ObservableCollection<RecentSellPriceModel>> GetRecentSellPricesAsync(int MaxRecord, int CustomerId, int ProductId)
        {
            var sql = string.Format(GetRecentPricesOfProduct, MaxRecord, CustomerId, ProductId);
            return await DataAccess.LoadDataAsync<RecentSellPriceModel, DynamicParameters>(sql, null);
        }

        public ValidationResult ValidateItem(InvoiceModel item)
        {
            InvoiceValidator validator = new();
            var result = validator.Validate(item);
            return result;
        }

        public ValidationResult ValidateItem(InvoiceItemModel item)
        {
            InvoiceItemValidator validator = new();
            var result = validator.Validate(item);
            return result;
        }

        public ValidationResult ValidateItem(InvoicePaymentModel item)
        {
            InvoicePaymentValidator validator = new();
            var result = validator.Validate(item);
            return result;
        }
    }

    internal class CustomerModelMapper : EntityMap<CustomerModel>
    {
        public CustomerModelMapper()
        {
            Map(x => x.Id).ToColumn("CustId");
            Map(x => x.Descriptions).ToColumn("CustDescriptions");
        }
    }
    internal class ProductModelMapper : EntityMap<ProductModel>
    {
        public ProductModelMapper()
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
    }
    internal class ProductUnitModelMapper : EntityMap<ProductUnitModel>
    {
        public ProductUnitModelMapper()
        {
            Map(x => x.Id).ToColumn("puId");
        }
    }
}