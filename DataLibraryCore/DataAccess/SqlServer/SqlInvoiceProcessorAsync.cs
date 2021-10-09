using Dapper;
using DataLibraryCore.DataAccess.Interfaces;
using DataLibraryCore.Models;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.SqlServer
{
    public partial class SqlInvoiceProcessor : IInvoiceProcessor
    {
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
            item.DateUpdated = PersianCalendarModel.GetCurrentPersianDate();
            item.TimeUpdated = PersianCalendarModel.GetCurrentTime();
            return await DataAccess.SaveDataAsync(UpdateInvoiceQuery, item);
        }

        public async Task<InvoiceItemModel> GetInvoiceItemFromDatabaseAsync(int Id)
        {
            var sql = string.Format(GetSingleInvoiceItemQuery, Id);
            using IDbConnection conn = new SqlConnection(DataAccess.GetConnectionString());
            var result = await conn.QueryAsync<InvoiceItemModel, ProductModel, InvoiceItemModel>
                (sql, (it, p) => { it.Product = p; return it; }, splitOn: "pId");
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
            var InvoiceId = GetInvoiceIdFromInvoiceItemId(ItemId);
            if (InvoiceId == 0) return 0;
            var AffectedCount = await DataAccess.SaveDataAsync(DeleteInvoiceItemQuery, ItemId);
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
            var InvoiceId = GetInvoiceIdFromInvoicePaymentId(PaymentId);
            if (InvoiceId == 0) return 0;
            var AffectedCount = await DataAccess.SaveDataAsync(DeleteInvoicePaymentQuery, PaymentId);
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

        public async Task<int> DeleteItemByIdAsync(int Id)
        {
            string sql = @$"DELETE FROM Invoices WHERE Id = {Id}";
            return await DataAccess.SaveDataAsync<DynamicParameters>(sql, null);
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
                            SELECT i.Id, i.CustomerId, c.FirstName + ' ' + c.LastName CustomerFullName, i.DateCreated, i.DateUpdated,
		                            dbo.GetDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue) AS TotalInvoiceSum,
		                            pays.TotalPayments, i.LifeStatus
                            FROM Invoices i LEFT JOIN Customers c ON i.CustomerId = c.Id
                            
                            LEFT JOIN (SELECT SUM(ii.[CountValue] * ii.SellPrice) AS TotalSellValue, ii.[InvoiceId]
	                            FROM InvoiceItems ii GROUP BY ii.[InvoiceId]) sp ON i.Id=sp.InvoiceId
                            
                            LEFT JOIN (SELECT SUM(ips.[PayAmount]) AS TotalPayments, ips.[InvoiceId]
	                           FROM InvoicePayments ips GROUP BY ips.[InvoiceId]) pays ON i.Id=pays.InvoiceId

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
            string InvoiceClause = InvoiceId == 0 ? "" : $"AND [Id] <> { InvoiceId }";
            string sqlQuery = @$"SET NOCOUNT ON
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

        public async Task<List<ProductNamesForComboBox>> GetProductItemsAsync(string SearchText = null)
        {
            var where = string.IsNullOrEmpty(SearchText) ? "" : $" WHERE [ProductName] LIKE '%{ SearchText }%'";
            var sql = string.Format(GetProductItemsQuery, where);
            using IDbConnection conn = new SqlConnection(DataAccess.GetConnectionString());
            var items = await conn.QueryAsync<ProductNamesForComboBox>(sql, null);
            return items?.ToList();
        }
    }
}