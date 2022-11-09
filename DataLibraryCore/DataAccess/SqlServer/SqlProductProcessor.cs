using Dapper;
using System.Data;
using System.Linq;
using System.Collections.ObjectModel;
using FluentValidation.Results;
using DataLibraryCore.DataAccess.Interfaces;
using SharedLibrary.Validators;
using SharedLibrary.DalModels;
using SharedLibrary.Enums;
using System.Threading.Tasks;
using SharedLibrary.Helpers;

namespace DataLibraryCore.DataAccess.SqlServer
{
    public class SqlProductProcessor<TModel, TValidator> : IGeneralProcessor<TModel>
        where TModel : ProductModel where TValidator : ProductValidator, new()
    {
        public SqlProductProcessor(IDataAccess dataAcess)
        {
            DataAccess = dataAcess;
        }

        private readonly IDataAccess DataAccess;
        private const string QueryOrderBy = "ProductName";
        private const OrderType QueryOrderType = OrderType.ASC;
        private readonly string CreateProductQuery = @"DECLARE @newId int; SET @newId = (SELECT ISNULL(MAX([Id]), 0) FROM [Products]) + 1;
            INSERT INTO Products ([Id], ProductName, BuyPrice, SellPrice, Barcode, CountString, DateCreated, TimeCreated, Descriptions, IsActive)
            VALUES (@newId, @productName, @buyPrice, @sellPrice, @barcode, @countString, @dateCreated, @timeCreated, @descriptions, @isActive);
            SELECT @id = @newId;";
        private readonly string UpdateProductQuery = @"UPDATE Products SET ProductName = @productName, BuyPrice = @buyPrice, SellPrice = @sellPrice, Barcode = @barcode,
            CountString = @countString, DateUpdated = @dateUpdated, TimeUpdated = @timeUpdated, Descriptions = @descriptions, IsActive = @isActive
            WHERE Id = @id";
        private readonly string DeleteProductQuery = @"DELETE FROM Products WHERE Id = @id";

        public string GenerateWhereClause(string val, SqlSearchMode mode)
        {
            var criteria = string.IsNullOrWhiteSpace(val) ? "'%'" : $"'%{ val }%'";
            return @$"(CAST([Id] AS varchar) LIKE {criteria}
                      {mode} [ProductName] LIKE N{criteria}
                      {mode} CAST([BuyPrice] AS varchar) LIKE {criteria}
                      {mode} CAST([SellPrice] AS varchar) LIKE {criteria}
                      {mode} [Barcode] LIKE {criteria}
                      {mode} [CountString] LIKE N{criteria}
                      {mode} [DateCreated] LIKE {criteria}
                      {mode} [TimeCreated] LIKE {criteria}
                      {mode} [DateUpdated] LIKE {criteria}
                      {mode} [TimeUpdated] LIKE {criteria}
                      {mode} [Descriptions] LIKE N{criteria} )";
        }

        public ValidationResult ValidateItem(TModel product)
        {
            TValidator validator = new();
            var result = validator.Validate(product);
            return result;
        }

        public async Task<int> CreateItemAsync(TModel item)
        {
            if (item == null || !ValidateItem(item).IsValid) return 0;
            item.DateCreated = PersianCalendarHelper.GetCurrentPersianDate();
            item.TimeCreated = PersianCalendarHelper.GetCurrentTime();
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
            dp.Add("@isActive", item.IsActive);
            var AffectedCount = await DataAccess.SaveDataAsync(CreateProductQuery, dp);
            var OutputId = dp.Get<int>("@id");
            if (AffectedCount > 0) item.Id = OutputId;
            return OutputId;
        }

        public async Task<int> UpdateItemAsync(TModel item)
        {
            if (item == null || !ValidateItem(item).IsValid) return 0;
            item.DateUpdated = PersianCalendarHelper.GetCurrentPersianDate();
            item.TimeUpdated = PersianCalendarHelper.GetCurrentTime();
            return await DataAccess.SaveDataAsync(UpdateProductQuery, item);
        }

        public async Task<int> DeleteItemByIdAsync(int Id)
        {
            DynamicParameters dp = new();
            dp.Add("@id", Id);
            return await DataAccess.SaveDataAsync(DeleteProductQuery, dp);
        }

        public async Task<int> GetTotalQueryCountAsync(string WhereClause)
        {
            var sqlTemp = $@"SELECT COUNT([Id]) FROM Products
                                { (string.IsNullOrEmpty(WhereClause) ? "" : " WHERE ") } { WhereClause }";
            return await DataAccess.ExecuteScalarAsync<int, DynamicParameters>(sqlTemp, null);
        }

        public async Task<ObservableCollection<TModel>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, string OrderBy = QueryOrderBy, OrderType Order = QueryOrderType)
        {
            string sql = $@"SET NOCOUNT ON
                            SELECT * FROM Products
                            { (string.IsNullOrEmpty(WhereClause) ? "" : $" WHERE { WhereClause }") }
                            ORDER BY [{OrderBy}] {Order} OFFSET {OffSet} ROWS FETCH NEXT {FetcheSize} ROWS ONLY";
            return await DataAccess.LoadDataAsync<TModel, DynamicParameters>(sql, null);
        }

        public async Task<TModel> LoadSingleItemAsync(int Id)
        {
            var outPut = await LoadManyItemsAsync(0, 1, $"[Id] = { Id }");
            return outPut.FirstOrDefault();
        }
    }
}