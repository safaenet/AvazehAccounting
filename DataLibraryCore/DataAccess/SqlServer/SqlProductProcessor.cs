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
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.SqlServer
{
    public partial class SqlProductProcessor<TModel, TValidator> : IProcessor<TModel>
        where TModel : ProductModel where TValidator : ProductValidator, new()
    {
        public SqlProductProcessor(IDataAccess dataAcess)
        {
            DataAccess = dataAcess;
        }

        private readonly IDataAccess DataAccess;
        private const string QueryOrderBy = "ProductName";
        private const OrderType QueryOrderType = OrderType.ASC;
        private readonly string CreateProductQuery = @"INSERT INTO Products (ProductName, BuyPrice, SellPrice, Barcode, CountString, DateCreated, TimeCreated, Descriptions)
            VALUES (@productName, @buyPrice, @sellPrice, @barcode, @countString, @dateCreated, @timeCreated, @descriptions);
            SELECT @id = @@IDENTITY;";
        private readonly string UpdateProductQuery = @"UPDATE Products SET ProductName = @productName, BuyPrice = @buyPrice, SellPrice = @sellPrice, Barcode = @barcode,
            CountString = @countString, DateUpdated = @dateUpdated, TimeUpdated = @timeUpdated, Descriptions = @descriptions
            WHERE Id = @id";
        private readonly string DeleteProductQuery = @"DELETE FROM Products WHERE Id = @id";

        public int CreateItem(TModel item)
        {
            if (item == null || !ValidateItem(item).IsValid) return 0;
            item.DateCreated = PersianCalendarModel.GetCurrentPersianDate();
            item.TimeCreated = PersianCalendarModel.GetCurrentTime();
            var dp = new DynamicParameters();
            dp.Add("@id", 0, DbType.Int32, ParameterDirection.Output);
            dp.Add("@productName", item.ProductName);
            dp.Add("@buyPrice", item.BuyPrice);
            dp.Add("@sellPrice", item.SellPrice);
            dp.Add("@barcode", item.Barcode);
            dp.Add("@countString", item.CountString);
            dp.Add("@dateCreated", item.DateCreated);
            dp.Add("@timeCreated", item.TimeCreated);
            dp.Add("@descriptions", item.Descriptions);
            var AffectedCount = DataAccess.SaveData(CreateProductQuery, dp);
            var OutputId = dp.Get<int>("@id");
            if (AffectedCount > 0) item.Id = OutputId;
            return OutputId;
        }

        public int UpdateItem(TModel item)
        {
            if (item == null || !ValidateItem(item).IsValid) return 0;
            item.DateUpdated = PersianCalendarModel.GetCurrentPersianDate();
            item.TimeUpdated = PersianCalendarModel.GetCurrentTime();
            return DataAccess.SaveData(UpdateProductQuery, item);
        }

        public int DeleteItemById(int Id)
        {
            DynamicParameters dp = new();
            dp.Add("@id", Id);
            return DataAccess.SaveData(DeleteProductQuery, dp);
        }

        public int GetTotalQueryCount(string WhereClause)
        {
            var sqlTemp = $@"SELECT COUNT([Id]) FROM Products
                                { (string.IsNullOrEmpty(WhereClause) ? "" : " WHERE ") } { WhereClause }";
            return DataAccess.ExecuteScalar<int, DynamicParameters>(sqlTemp, null);
        }

        public ObservableCollection<TModel> LoadManyItems(int OffSet, int FetcheSize, string WhereClause, string OrderBy = QueryOrderBy, OrderType Order = QueryOrderType)
        {
            string sql = $@"SET NOCOUNT ON
                            SELECT * FROM Products
                            { (string.IsNullOrEmpty(WhereClause) ? "" : $" WHERE { WhereClause }") }
                            ORDER BY [{OrderBy}] {Order} OFFSET {OffSet} ROWS FETCH NEXT {FetcheSize} ROWS ONLY";
            return DataAccess.LoadData<TModel, DynamicParameters>(sql, null);
        }

        public TModel LoadSingleItem(int Id)
        {
            return LoadManyItems(0, 1, $"[Id] = { Id }").FirstOrDefault();
        }

        public string GenerateWhereClause(string val, SqlSearchMode mode)
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

        public ValidationResult ValidateItem(TModel product)
        {
            TValidator validator = new();
            var result = validator.Validate(product);
            return result;
        }
    }
}