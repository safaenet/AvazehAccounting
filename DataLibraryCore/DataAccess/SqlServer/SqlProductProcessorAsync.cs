using Dapper;
using DataLibraryCore.DataAccess.Interfaces;
using DataLibraryCore.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.SqlServer
{
    public partial class SqlProductProcessor : IProductProcessor
    {
        public async Task<int> CreateItemAsync(ProductModel product)
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
            var AffectedCount = await DataAccess.SaveDataAsync(CreateProductQuery, dp);
            var OutputId = dp.Get<int>("@id");
            if (AffectedCount > 0) product.Id = OutputId;
            return OutputId;
        }

        public async Task<int> UpdateItemAsync(ProductModel product)
        {
            if (product == null) return 0;
            product.DateUpdated = PersianCalendarModel.GetCurrentPersianDate();
            product.TimeUpdated = PersianCalendarModel.GetCurrentTime();
            return await DataAccess.SaveDataAsync(UpdateProductQuery, product);
        }

        public async Task<int> DeleteItemByIdAsync(int ID)
        {
            DynamicParameters dp = new();
            dp.Add("@id", ID);
            return await DataAccess.SaveDataAsync(DeleteProductQuery, dp);
        }

        public async Task<int> GetTotalQueryCountAsync(string WhereClause)
        {
            var sqlTemp = $@"SELECT COUNT([Id]) FROM Products
                                { (string.IsNullOrEmpty(WhereClause) ? "" : " WHERE ") } { WhereClause }";
            return await DataAccess.ExecuteScalarAsync<int, DynamicParameters>(sqlTemp, null);
        }

        public async Task<ObservableCollection<ProductModel>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, OrderType Order = OrderType.ASC, string OrderBy = "Id")
        {
            string sql = $@"SET NOCOUNT ON
                            SELECT * FROM Products
                            { (string.IsNullOrEmpty(WhereClause) ? "" : $" WHERE { WhereClause }") }
                            ORDER BY [{OrderBy}] {Order} OFFSET {OffSet} ROWS FETCH NEXT {FetcheSize} ROWS ONLY";
            return await DataAccess.LoadDataAsync<ProductModel, DynamicParameters>(sql, null);
        }

        public async Task<ProductModel> LoadSingleItemAsync(int ID)
        {
            var outPut = await LoadManyItemsAsync(0, 1, $"[Id] = { ID }");
            return outPut.FirstOrDefault();
        }

        public async Task<string> GenerateWhereClauseAsync(string val, SqlSearchMode mode = SqlSearchMode.OR)
        {
            var criteria = string.IsNullOrWhiteSpace(val) ? "'%'" : $"'%{ val }%'";
            var outPut = @$"(CAST([Id] AS varchar) LIKE {criteria}
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
            return await Task.FromResult(outPut);
        }
    }
}