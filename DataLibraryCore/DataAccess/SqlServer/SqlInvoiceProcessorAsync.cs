using Dapper;
using Dapper.FluentMap;
using DataLibraryCore.DataAccess.Interfaces;
using DataLibraryCore.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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

        public async Task<int> InsertSubItemToDatabaseAsync(InvoiceItemModel item)
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
            dp.Add("@dateCreated", item.DateCreated);
            dp.Add("@timeCreated", item.TimeCreated);
            dp.Add("@delivered", item.Delivered);
            dp.Add("@descriptions", item.Descriptions);
            var AffectedCount = await DataAccess.SaveDataAsync(InsertSubItemQuery, dp);
            if (AffectedCount > 0)
            {
                item.Id = dp.Get<int>("@id");
                UpdateItemUpdateDateAndUpdateTimeAsync(item.InvoiceId);
            }
            return AffectedCount;
        }

        public async Task<int> UpdateSubItemInDatabaseAsync(InvoiceItemModel item)
        {
            if (item == null || !item.IsCountStringValid) return 0;
            item.DateUpdated = PersianCalendarModel.GetCurrentPersianDate();
            item.TimeUpdated = PersianCalendarModel.GetCurrentTime();
            DynamicParameters dp = new();
            dp.Add("@productId", item.Product.Id);
            dp.Add("@buyPrice", item.BuyPrice);
            dp.Add("@sellPrice", item.SellPrice);
            dp.Add("@countString", item.CountString);
            dp.Add("@countValue", item.CountValue);
            dp.Add("@dateUpdated", item.DateUpdated);
            dp.Add("@timeUpdated", item.TimeUpdated);
            dp.Add("@delivered", item.Delivered);
            dp.Add("@descriptions", item.Descriptions);
            var AffectedCount = await DataAccess.SaveDataAsync(UpdateSubItemQuery, item);
            if (AffectedCount > 0) UpdateItemUpdateDateAndUpdateTimeAsync(item.InvoiceId);
            return AffectedCount;
        }

        public async Task<int> DeleteSubItemFromDatabaseAsync(InvoiceItemModel item)
        {
            var AffectedCount = await DataAccess.SaveDataAsync(DeleteSubItemQuery, item);
            if (AffectedCount > 0) UpdateItemUpdateDateAndUpdateTimeAsync(item.InvoiceId);
            return AffectedCount;
        }

        private async void UpdateItemUpdateDateAndUpdateTimeAsync(int Id)
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
            var sqlTemp = $@"SELECT COUNT([Id]) FROM Invoices
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
            if (FluentMapper.EntityMaps.IsEmpty)
            {
                FluentMapper.Initialize(config => config.AddMap(new CustomerModelMapper()));
                FluentMapper.Initialize(config => config.AddMap(new ProductModelMapper()));
            }
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

        public async Task<Dictionary<int, string>> GetProductItemsAsync()
        {
            Dictionary<int, string> choices = new();
            string sql = $@"SELECT p.Id, p.ProductName FROM Products p";
            using IDbConnection conn = new SqlConnection(DataAccess.GetConnectionString());
            var items = await conn.QueryAsync<ProductNamesForComboBox>(sql, null);
            choices = items.ToDictionary(x => x.Id, x => x.ProductName);
            return choices;
        }
    }
}