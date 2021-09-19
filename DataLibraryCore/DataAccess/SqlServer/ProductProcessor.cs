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
using DataLibraryCore.DataAccess.Interfaces;

namespace DataLibraryCore.DataAccess.SqlServer
{
    public class SqlProductProcessor : IProductProcessor
    {
        public SqlProductProcessor(IDataAccess dataAcess)
        {
            DataAccess = dataAcess;
        }

        private readonly IDataAccess DataAccess;
        private readonly string CreateProductQuery = @"INSERT INTO Products (ProductName, BuyPrice, SellPrice, Barcode, CountString, DateCreated, TimeCreated, Descriptions)
            VALUES (@productName, @buyPrice, @sellPrice, @barcode, @countString, @dateCreated, @timeCreated, @descriptions);
            SELECT @id = @@IDENTITY;";
        private readonly string UpdateProductQuery = @"UPDATE Products SET ProductName = @productName, BuyPrice = @buyPrice, SellPrice = @sellPrice, Barcode = @barcode,
            CountString = @countString, DateUpdated = @dateUpdated, TimeUpdated = @timeUpdated, Descriptions = @descriptions
            WHERE Id = @id";
        private readonly string DeleteProductQuery = @"DELETE FROM Products WHERE Id = @id";

        public int CreateItem(ProductModel product)
        {
            if (product == null) return 0;
            product.DateCreated = PersianCalendarModel.GetCurrentPersianDate();
            product.TimeCreated = PersianCalendarModel.GetCurrentTime();
            var dp = new DynamicParameters();
            dp.Add("@id", 0, DbType.Int32, ParameterDirection.Output);
            dp.Add("@productName", product.ProductName);
            dp.Add("@buyPrice", product.BuyPrice);
            dp.Add("@sellPrice", product.SellPrice);
            dp.Add("@barcode", product.Barcode);
            dp.Add("@countString", product.CountString);
            dp.Add("@dateCreated", product.DateCreated);
            dp.Add("@timeCreated", product.TimeCreated);
            dp.Add("@descriptions", product.Descriptions);
            var AffectedCount = DataAccess.SaveData(CreateProductQuery, dp);
            var OutputId = dp.Get<int>("@id");
            if (AffectedCount > 0) product.Id = OutputId;
            return OutputId;
        }

        public int UpdateItem(ProductModel product)
        {
            if (product == null) return 0;
            product.DateUpdated = PersianCalendarModel.GetCurrentPersianDate();
            product.TimeUpdated = PersianCalendarModel.GetCurrentTime();
            return DataAccess.SaveData(UpdateProductQuery, product);
        }

        public int DeleteItemById(int ID)
        {
            DynamicParameters dp = new();
            dp.Add("@id", ID);
            return DataAccess.SaveData(DeleteProductQuery, dp);
        }

        public int GetTotalQueryCount(string WhereClause)
        {
            var sqlTemp = $@"SELECT COUNT([Id]) FROM Products
                                { (string.IsNullOrEmpty(WhereClause) ? "" : " WHERE ") } { WhereClause }";
            return DataAccess.ExecuteScalar<int, DynamicParameters>(sqlTemp, null);
        }

        public ObservableCollection<ProductModel> LoadManyItems(int OffSet, int FetcheSize, string WhereClause, OrderType Order = OrderType.ASC, string OrderBy = "Id")
        {
            string sql = $@"SET NOCOUNT ON
                            SELECT * FROM Products
                            { (string.IsNullOrEmpty(WhereClause) ? "" : $" WHERE { WhereClause }") }
                            ORDER BY [{OrderBy}] {Order} OFFSET {OffSet} ROWS FETCH NEXT {FetcheSize} ROWS ONLY";
            return DataAccess.LoadData<ProductModel, DynamicParameters>(sql, null);
        }

        public ProductModel LoadSingleItem(int ID)
        {
            return LoadManyItems(0, 1, $"[Id] = { ID }").FirstOrDefault();
        }

        public string GenerateWhereClause(string val, SqlSearchMode mode = SqlSearchMode.OR)
        {
            var criteria = string.IsNullOrWhiteSpace(val) ? "'%'" : $"'%{ val }%'";
            return @$"(CAST([Id] AS varchar) LIKE {criteria}
                      {mode} [ProductName] LIKE {criteria}
                      {mode} CAST([BuyPrice] AS varchar) LIKE {criteria}
                      {mode} CAST([SellPrice] AS varchar) LIKE {criteria}
                      {mode} [Barcode] LIKE {criteria}
                      {mode} [CountString] LIKE {criteria}
                      {mode} [DateCreated] LIKE {criteria}
                      {mode} [TimeCreated] LIKE {criteria}
                      {mode} [DateUpdated] LIKE {criteria}
                      {mode} [TimeUpdated] LIKE {criteria}
                      {mode} [Descriptions] LIKE {criteria} )";
        }

        public ValidationResult ValidateItem(ProductModel product)
        {
            ProductValidator validator = new();
            var result = validator.Validate(product);
            return result;
        }
    }
}