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
        public async Task<int> CreateItemAsync(InvoiceModel invoice)
        {
            if (invoice == null) return 0;
            invoice.DateCreated = PersianCalendarModel.GetCurrentPersianDate();
            invoice.TimeCreated = PersianCalendarModel.GetCurrentTime();
            var dp = new DynamicParameters();
            dp.Add("@id", 0, DbType.Int32, ParameterDirection.Output);
            dp.Add("@customerId", invoice.Customer.Id);
            dp.Add("@dateCreated", invoice.DateCreated);
            dp.Add("@timeCreated", invoice.TimeCreated);
            dp.Add("@discountType", invoice.DiscountType);
            dp.Add("@discountValue", invoice.DiscountValue);
            dp.Add("@descriptions", invoice.Descriptions);
            dp.Add("@lifeStatus", invoice.LifeStatus);
            var AffectedCount = await DataAccess.SaveDataAsync(CreateInvoiceQuery, dp);
            var OutputId = dp.Get<int>("@id");
            if (AffectedCount > 0) invoice.Id = OutputId;
            return OutputId;
        }

        public async Task<int> UpdateItemAsync(InvoiceModel invoice)
        {
            if (invoice == null) return 0;
            invoice.DateUpdated = PersianCalendarModel.GetCurrentPersianDate();
            invoice.TimeUpdated = PersianCalendarModel.GetCurrentTime();
            return await DataAccess.SaveDataAsync(UpdateInvoiceQuery, invoice);
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

        private async void UpdateItemUpdateDateAndUpdateTimeAsync(int ID)
        {
            var dp = new DynamicParameters();
            dp.Add("@id", ID);
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

        public async Task<ObservableCollection<InvoiceListModel>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, OrderType Order = OrderType.DESC, string OrderBy = "Id")
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

        public async Task<InvoiceModel> LoadSingleItemAsync(int ID)
        {
            var query = string.Format(LoadSingleItemQuery, ID);
            if (FluentMapper.EntityMaps.IsEmpty)
            {
                FluentMapper.Initialize(config => config.AddMap(new CustomerModelMapper()));
                FluentMapper.Initialize(config => config.AddMap(new ProductModelMapper()));
            }
            using IDbConnection conn = new SqlConnection(DataAccess.GetConnectionString());
            var outPut = await conn.QueryMultipleAsync(query);
            return outPut.MapToSingleInvoice();
        }

        public async Task<double> GetTotalOrRestTotalBalanceOfCustomerAsync(int CustomerID, int InvoiceID = 0)
        {
            string InvoiceClause = InvoiceID == 0 ? "" : $"AND [Id] <> { InvoiceID }";
            string sqlQuery = @$"SET NOCOUNT ON
                                 SELECT SUM(ISNULL(dbo.GetDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue), 0) - ISNULL(pays.TotalPayments, 0))
                                 FROM Invoices i LEFT JOIN Customers c ON i.CustomerId = c.Id
                                 
                                 LEFT JOIN (SELECT SUM(ii.[CountValue] * ii.[SellPrice]) AS TotalSellValue, ii.[InvoiceId]
                                     FROM InvoiceItems ii GROUP BY ii.[InvoiceId]) sp ON i.Id=sp.InvoiceId
                                 
                                 LEFT JOIN (SELECT SUM(ips.[PayAmount]) AS TotalPayments, ips.[InvoiceId]
                                    FROM InvoicePayments ips GROUP BY ips.[InvoiceId]) pays ON i.Id=pays.InvoiceId
                                 WHERE i.LifeStatus = { (int)InvoiceLifeStatus.Active } AND c.Id = { CustomerID } { InvoiceClause }
                                 GROUP BY c.Id";
            return DataAccess.ExecuteScalar<double, DynamicParameters>(sqlQuery, null);
        }

        public async Task<Dictionary<int, string>> GetProductItemsAsync()
        {
            Dictionary<int, string> choices = new();
            string sql = $@"SELECT p.Id, p.ProductName FROM Products p";
            using IDbConnection conn = new SqlConnection(DataAccess.GetConnectionString());
            var items = conn.Query<ProductNamesForComboBox>(sql, null);
            choices = items.ToDictionary(x => x.Id, x => x.ProductName);
            return choices;
        }

        public async Task<string> GenerateWhereClauseAsync(string val, InvoiceLifeStatus? LifeStatus, InvoiceFinancialStatus? FinStatus, SqlSearchMode mode = SqlSearchMode.OR)
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
    }
}