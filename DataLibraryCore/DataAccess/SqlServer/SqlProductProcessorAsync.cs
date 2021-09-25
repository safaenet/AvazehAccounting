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
    public partial class SqlProductProcessor<TModel, TValidator> : IProcessor<TModel>
    {
        public async Task<int> CreateItemAsync(TModel item)
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
            var AffectedCount = await DataAccess.SaveDataAsync(CreateProductQuery, dp);
            var OutputId = dp.Get<int>("@id");
            if (AffectedCount > 0) item.Id = OutputId;
            return OutputId;
        }

        public async Task<int> UpdateItemAsync(TModel item)
        {
            if (item == null || !ValidateItem(item).IsValid) return 0;
            item.DateUpdated = PersianCalendarModel.GetCurrentPersianDate();
            item.TimeUpdated = PersianCalendarModel.GetCurrentTime();
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

        public async Task<ObservableCollection<TModel>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, OrderType Order = OrderType.ASC, string OrderBy = "Id")
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