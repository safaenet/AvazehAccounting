using Dapper;
using DataLibraryCore.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using DataLibraryCore.DataAccess.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.ObjectModel;
using DataLibraryCore.Models.Validators;
using FluentValidation.Results;
using System.Reflection;
using System.ComponentModel;
using Dapper.FluentMap.Mapping;
using Dapper.FluentMap;
using DataLibraryCore.DataAccess.Interfaces;

namespace DataLibraryCore.DataAccess.SqlServer
{
    public partial class SqlInvoiceProcessor : IInvoiceProcessor
    {
        public SqlInvoiceProcessor(IDataAccess dataAcess)
        {
            DataAccess = dataAcess;
        }

        private readonly IDataAccess DataAccess;
        private readonly string CreateInvoiceQuery = @"INSERT INTO Invoices (CustomerId, DateCreated, TimeCreated, DiscountType, DiscountValue, Descriptions, LifeStatus)
            VALUES (@customerId, @dateCreated, @timeCreated, @discountType, @discountValue, @descriptions, @lifeStatus); SELECT @id = @@IDENTITY;";
        private readonly string UpdateInvoiceQuery = @"UPDATE Invoices SET CustomerId = @customerId, DateCreated = @dateCreated, TimeCreated = @timeCreated,
            DateUpdated = @dateUpdated, TimeUpdated = @timeUpdated, DiscountType = @discountType,
            DiscountValue = @discountValue, Descriptions = @descriptions, LifeStatus = @lifeStatus WHERE Id = @id";
        private readonly string InsertSubItemQuery = @"INSERT INTO InvoiceItems (InvoiceId, ProductId, BuyPrice, SellPrice, CountString, CountValue, DateCreated,
            TimeCreated, Delivered, Descriptions)
            VALUES (@invoiceId, @productId, @buyPrice, @sellPrice, @countString, @dateCreated, @timeCreated, @delivered, @descriptions);
            SELECT @id = @@IDENTITY;";
        private readonly string UpdateSubItemQuery = @$"UPDATE InvoiceItems SET ProductId = @productId, BuyPrice = @buyPrice, SellPrice = @sellPrice,
            CountString = @countString, CountValue = @countValue, DateUpdated = @dateUpdated, TimeUpdated = @timeUpdated,
            Delivered = @delivered, Descriptions = @descriptions WHERE [Id] = @id";
        private readonly string UpdateSubItemDateAndTimeQuery = @"UPDATE Invoices SET DateUpdated = @dateUpdated, TimeUpdated = @timeUpdated WHERE Id = @id";
        private readonly string DeleteSubItemQuery = @$"DELETE FROM InvoiceItems WHERE [Id] = @id";
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
                p.TimeCreated pTimeCreated, p.DateUpdated pDateUpdated, p.TimeUpdated pTimeUpdated, p.Descriptions pDescriptions
                FROM InvoiceItems it LEFT JOIN Products p ON it.ProductId = p.Id WHERE it.InvoiceId IN (SELECT i.Id FROM @invoices i);
            SELECT * FROM InvoicePayments WHERE InvoiceId IN (SELECT i.Id FROM @invoices i);
            SELECT * FROM PhoneNumbers WHERE CustomerId IN (SELECT i.CustomerId FROM @invoices i);";

        public int CreateItem(InvoiceModel item)
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
            var AffectedCount = DataAccess.SaveData(CreateInvoiceQuery, dp);
            var OutputId = dp.Get<int>("@id");
            if (AffectedCount > 0) item.Id = OutputId;
            return OutputId;
        }

        public int UpdateItem(InvoiceModel item)
        {
            if (item == null || !ValidateItem(item).IsValid) return 0;
            item.DateUpdated = PersianCalendarModel.GetCurrentPersianDate();
            item.TimeUpdated = PersianCalendarModel.GetCurrentTime();
            return DataAccess.SaveData(UpdateInvoiceQuery, item);
        }

        public int InsertSubItemToDatabase(InvoiceItemModel item)
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
            var AffectedCount = DataAccess.SaveData(InsertSubItemQuery, dp);
            if (AffectedCount > 0)
            {
                item.Id = dp.Get<int>("@id");
                UpdateItemUpdateDateAndUpdateTime(item.InvoiceId);
            }
            return AffectedCount;
        }

        public int UpdateSubItemInDatabase(InvoiceItemModel item)
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
            var AffectedCount = DataAccess.SaveData(UpdateSubItemQuery, item);
            if (AffectedCount > 0) UpdateItemUpdateDateAndUpdateTime(item.InvoiceId);
            return AffectedCount;
        }

        public int DeleteSubItemFromDatabase(InvoiceItemModel item)
        {
            var AffectedCount = DataAccess.SaveData(DeleteSubItemQuery, item);
            if (AffectedCount > 0) UpdateItemUpdateDateAndUpdateTime(item.InvoiceId);
            return AffectedCount;
        }

        private void UpdateItemUpdateDateAndUpdateTime(int ID)
        {
            var dp = new DynamicParameters();
            dp.Add("@id", ID);
            dp.Add("@dateUpdated", PersianCalendarModel.GetCurrentPersianDate());
            dp.Add("@timeUpdated", PersianCalendarModel.GetCurrentTime());
            DataAccess.SaveData(UpdateSubItemDateAndTimeQuery, dp);
        }

        public int DeleteItemById(int Id)
        {
            string sql = @$"DELETE FROM Invoices WHERE Id = {Id}";
            return DataAccess.SaveData<DynamicParameters>(sql, null);
        }
        public int GetTotalQueryCount(string WhereClause)
        {
            var sqlTemp = $@"SELECT COUNT([Id]) FROM Invoices
                             { (string.IsNullOrEmpty(WhereClause) ? "" : " WHERE ") } { WhereClause }";
            return DataAccess.ExecuteScalar<int, DynamicParameters>(sqlTemp, null);
        }

        public ObservableCollection<InvoiceListModel> LoadManyItems(int OffSet, int FetcheSize, string WhereClause, OrderType Order = OrderType.DESC, string OrderBy = "Id")
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
            return DataAccess.LoadData<InvoiceListModel, DynamicParameters>(sql, null);
        }

        public InvoiceModel LoadSingleItem(int Id)
        {
            var query = string.Format(LoadSingleItemQuery, Id);
            if (FluentMapper.EntityMaps.IsEmpty)
            {
                FluentMapper.Initialize(config => config.AddMap(new CustomerModelMapper()));
                FluentMapper.Initialize(config => config.AddMap(new ProductModelMapper()));
            }
            using IDbConnection conn = new SqlConnection(DataAccess.GetConnectionString());
            return conn.QueryMultiple(query).MapToSingleInvoice();
        }

        public double GetTotalOrRestTotalBalanceOfCustomer(int CustomerId, int InvoiceId = 0)
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
            return DataAccess.ExecuteScalar<double, DynamicParameters>(sqlQuery, null);
        }

        public Dictionary<int, string> GetProductItems()
        {
            Dictionary<int, string> choices = new();
            string sql = $@"SELECT p.Id, p.ProductName FROM Products p";
            using IDbConnection conn = new SqlConnection(DataAccess.GetConnectionString());
            var items = conn.Query<ProductNamesForComboBox>(sql, null);
            choices = items.ToDictionary(x => x.Id, x => x.ProductName);
            return choices;
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

        public ValidationResult ValidateItem(InvoiceModel invoice)
        {
            InvoiceValidator validator = new();
            var result = validator.Validate(invoice);
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
}