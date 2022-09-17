using Dapper;
using DataLibraryCore.Models;
using System.Data;
using System.Linq;
using System.Collections.ObjectModel;
using FluentValidation.Results;
using DataLibraryCore.DataAccess.Interfaces;
using SharedLibrary.Validators;
using SharedLibrary.DalModels;
using SharedLibrary.Enums;
using System.Threading.Tasks;
using System.Data.SqlClient;
using SharedLibrary.DtoModels;
using System.Collections.Generic;

namespace DataLibraryCore.DataAccess.SqlServer
{
    public class SqlTransactionProcessor : ITransactionProcessor
    {
        public SqlTransactionProcessor(IDataAccess dataAcess)
        {
            DataAccess = dataAcess;
        }

        private readonly IDataAccess DataAccess;
        private const string QueryOrderBy = "Id";
        private const OrderType QueryOrderType = OrderType.ASC;
        private readonly string CreateTransactionQuery = @"DECLARE @newId int; SET @newId = (SELECT ISNULL(MAX([Id]), 0) FROM [Transactions]) + 1;
            INSERT INTO Transactions ([Id], FileName, DateCreated, TimeCreated, Descriptions)
            VALUES (@newId, @fileName, @dateCreated, @timeCreated, @descriptions);
            SELECT @id = @newId;";
        private readonly string UpdateTransactionQuery = @"UPDATE Transactions SET FileName = @fileName, DateCreated = @dateCreated, TimeCreated = @timeCreated, DateUpdated = @dateUpdated, TimeUpdated = @timeUpdated,
            Descriptions = @descriptions WHERE Id = @id";
        private readonly string DeleteTransactionQuery = @"DELETE FROM Transactions WHERE Id = @id";
        private readonly string GetSingleTransactionItemQuery = "SELECT * FROM TransactionItems WHERE [Id] = {0}";
        private readonly string InsertTransactionItemQuery = @"DECLARE @newId int; SET @newId = (SELECT ISNULL(MAX([Id]), 0) FROM [TransactionItems]) + 1;
            INSERT INTO TransactionItems ([Id], TransactionId, Title, Amount, CountString, CountValue, DateCreated, TimeCreated, Descriptions)
            VALUES (@newId, @transactionId, @title, @amount, @countString, @countValue, @dateCreated, @timeCreated, @descriptions);
            SELECT @id = @newId;";
        private readonly string UpdateTransactionItemQuery = @"UPDATE TransactionItems SET Title = @title, Amount = @amount,
            CountString = @countString, CountValue = @countValue, DateUpdated = @dateUpdated, TimeUpdated = @timeUpdated, Descriptions = @descriptions WHERE [Id] = @id";
        private readonly string DeleteTransactionItemQuery = @$"DELETE FROM TransactionItems WHERE [Id] = @id";
        private readonly string GetProductItemsQuery = "SELECT [Id], [ProductName] AS ItemName FROM Products {0} UNION SELECT [Id], [Title] AS ItemName FROM TransactionItems WHERE 1=1 {1} {2}";
        private readonly string GetTransactionNamesQuery = "SELECT [Id], [FileName] AS ItemName FROM Transactions {0}";
        private readonly string UpdateSubItemDateAndTimeQuery = @"UPDATE Transactions SET DateUpdated = @dateUpdated, TimeUpdated = @timeUpdated WHERE [Id] = @id";
        private readonly string LoadSingleItemQuery = @"SET NOCOUNT ON SELECT * FROM Transactions t WHERE t.[Id] = {0} ORDER BY t.[Id] DESC";

        private async Task<int> GetTransactionIdFromTransactionItemId(int Id)
        {
            var query = $"SELECT TransactionId FROM TransactionItems WHERE Id = { Id }";
            return await DataAccess.ExecuteScalarAsync<int, DynamicParameters>(query, null);
        }

        public string GenerateWhereClause(string val, TransactionFinancialStatus? FinStatus, SqlSearchMode mode) //Used for TransactionList
        {
            string finStatusOperand = "";
            switch (FinStatus)
            {
                case TransactionFinancialStatus.Balanced:
                    finStatusOperand = "=";
                    break;
                case TransactionFinancialStatus.Negative:
                    finStatusOperand = "<";
                    break;
                case TransactionFinancialStatus.Positive:
                    finStatusOperand = ">";
                    break;
                default:
                    break;
            }
            var criteria = string.IsNullOrWhiteSpace(val) ? "'%'" : $"'%{ val }%'";
            return @$"(CAST(t.[Id] AS VARCHAR) LIKE {criteria}
                      {mode} t.[FileName] LIKE {criteria}
                      {mode} t.[DateCreated] LIKE {criteria}
                      {mode} t.[TimeCreated] LIKE {criteria}
                      {mode} t.[DateUpdated] LIKE {criteria}
                      {mode} t.[TimeUpdated] LIKE {criteria}
                      {mode} t.[Descriptions] LIKE {criteria}
                      {mode} CAST(pos.TotalVal AS VARCHAR) LIKE {criteria}
					  {mode} CAST(neg.TotalVal AS VARCHAR) LIKE {criteria} )
                      { (FinStatus == null ? "" : $" AND ISNULL(pos.TotalVal, 0) + ISNULL(neg.TotalVal, 0) { finStatusOperand } 0 ")}
            ";
        }

        public string GenerateTransactionItemWhereClause(string val, TransactionFinancialStatus? FinStatus, SqlSearchMode mode) //Used for TransactionItemList
        {
            string finStatusOperand = "";
            switch (FinStatus)
            {
                case TransactionFinancialStatus.Balanced:
                    finStatusOperand = "=";
                    break;
                case TransactionFinancialStatus.Negative:
                    finStatusOperand = "<";
                    break;
                case TransactionFinancialStatus.Positive:
                    finStatusOperand = ">";
                    break;
                default:
                    break;
            }
            var criteria = string.IsNullOrWhiteSpace(val) ? "'%'" : $"'%{ val }%'";
            return @$"([Title] LIKE {criteria}
                      {mode} CAST([Amount] AS VARCHAR) LIKE {criteria}
                      {mode} [CountString] LIKE {criteria}
                      {mode} CAST([CountValue] AS VARCHAR) LIKE {criteria}
                      {mode} CAST(ISNULL(([Amount] * [CountValue]), 0) AS VARCHAR) LIKE {criteria}
                      {mode} [DateCreated] LIKE {criteria}
                      {mode} [TimeCreated] LIKE {criteria}
                      {mode} [DateUpdated] LIKE {criteria}
                      {mode} [TimeUpdated] LIKE {criteria}
                      {mode} [Descriptions] LIKE {criteria} ) 
                      { (FinStatus == null ? "" : $" AND ISNULL(([Amount] * [CountValue]), 0) { finStatusOperand } 0 ")}
            ";
        }

        public ValidationResult ValidateItem(TransactionModel transaction)
        {
            TransactionValidator validator = new();
            var result = validator.Validate(transaction);
            return result;
        }

        public async Task<List<ItemsForComboBox>> GetProductItemsAsync(string SearchText = null, int TransactionId = 0)
        {
            var where1 = string.IsNullOrEmpty(SearchText) ? "" : $" WHERE [ProductName] LIKE '%{ SearchText }%'";
            var where2 = string.IsNullOrEmpty(SearchText) ? "" : $" AND [Title] LIKE '%{ SearchText }%'";
            var where3 = TransactionId == 0 ? "" : $" AND [TransactionId] = { TransactionId }";
            var sql = string.Format(GetProductItemsQuery, where1, where2, where3);
            var items = await DataAccess.LoadDataAsync<ItemsForComboBox, DynamicParameters>(sql, null);
            return items?.ToList();
        }

        public async Task<List<ItemsForComboBox>> GetTransactionNamesAsync(string SearchText = null)
        {
            var where = string.IsNullOrEmpty(SearchText) ? "" : $" WHERE [Id] <> { SearchText }";
            var sql = string.Format(GetTransactionNamesQuery, where);
            var items = await DataAccess.LoadDataAsync<ItemsForComboBox, DynamicParameters>(sql, null);
            return items?.ToList();
        }

        public ValidationResult ValidateItem(TransactionItemModel item)
        {
            TransactionItemValidator validator = new();
            var result = validator.Validate(item);
            return result;
        }

        public async Task<int> CreateItemAsync(TransactionModel item)
        {
            if (item == null || !ValidateItem(item).IsValid) return 0;
            item.DateCreated = PersianCalendarModel.GetCurrentPersianDate();
            item.TimeCreated = PersianCalendarModel.GetCurrentTime();
            var dp = new DynamicParameters();
            dp.Add("@id", 0, DbType.Int32, ParameterDirection.Output);
            dp.Add("@fileName", item.FileName);
            dp.Add("@dateCreated", item.DateCreated);
            dp.Add("@timeCreated", item.TimeCreated);
            dp.Add("@descriptions", item.Descriptions);
            var AffectedCount = await DataAccess.SaveDataAsync(CreateTransactionQuery, dp);
            var OutputId = dp.Get<int>("@id");
            if (AffectedCount > 0) item.Id = OutputId;
            return OutputId;
        }

        public async Task<int> UpdateItemAsync(TransactionModel item)
        {
            if (item == null || !ValidateItem(item).IsValid) return 0;
            item.DateUpdated = PersianCalendarModel.GetCurrentPersianDate();
            item.TimeUpdated = PersianCalendarModel.GetCurrentTime();
            return await DataAccess.SaveDataAsync(UpdateTransactionQuery, item);
        }

        public async Task<int> DeleteItemByIdAsync(int Id)
        {
            DynamicParameters dp = new();
            dp.Add("@id", Id);
            return await DataAccess.SaveDataAsync(DeleteTransactionQuery, dp);
        }

        public async Task<TransactionItemModel> GetTransactionItemFromDatabaseAsync(int Id)
        {
            var sql = string.Format(GetSingleTransactionItemQuery, Id);
            using IDbConnection conn = new SqlConnection(DataAccess.GetConnectionString());
            var result = await conn.QueryAsync<TransactionItemModel>(sql);
            return result.SingleOrDefault();
        }

        public async Task<int> InsertTransactionItemToDatabaseAsync(TransactionItemModel item)
        {
            if (item == null || !item.IsCountStringValid) return 0;
            item.DateCreated = PersianCalendarModel.GetCurrentPersianDate();
            item.TimeCreated = PersianCalendarModel.GetCurrentTime();
            DynamicParameters dp = new();
            dp.Add("@id", 0, DbType.Int32, ParameterDirection.Output);
            dp.Add("@transactionId", item.TransactionId);
            dp.Add("@title", item.Title);
            dp.Add("@amount", item.Amount);
            dp.Add("@countString", item.CountString);
            dp.Add("@countValue", item.CountValue);
            dp.Add("@dateCreated", item.DateCreated);
            dp.Add("@timeCreated", item.TimeCreated);
            dp.Add("@descriptions", item.Descriptions);
            var AffectedCount = await DataAccess.SaveDataAsync(InsertTransactionItemQuery, dp);
            if (AffectedCount > 0)
            {
                item.Id = dp.Get<int>("@id");
                await UpdateItemUpdateDateAndUpdateTimeAsync(item.TransactionId);
            }
            return AffectedCount;
        }

        public async Task<int> UpdateTransactionItemInDatabaseAsync(TransactionItemModel item)
        {
            if (item == null || !item.IsCountStringValid) return 0;
            item.DateUpdated = PersianCalendarModel.GetCurrentPersianDate();
            item.TimeUpdated = PersianCalendarModel.GetCurrentTime();
            DynamicParameters dp = new();
            dp.Add("@id", item.Id);
            dp.Add("@title", item.Title);
            dp.Add("@amount", item.Amount);
            dp.Add("@countString", item.CountString);
            dp.Add("@countValue", item.CountValue);
            dp.Add("@dateUpdated", item.DateUpdated);
            dp.Add("@timeUpdated", item.TimeUpdated);
            dp.Add("@descriptions", item.Descriptions);
            var AffectedCount = await DataAccess.SaveDataAsync(UpdateTransactionItemQuery, dp);
            if (AffectedCount > 0) await UpdateItemUpdateDateAndUpdateTimeAsync(item.TransactionId);
            return AffectedCount;
        }

        public async Task<int> DeleteTransactionItemFromDatabaseAsync(int ItemId)
        {
            var InvoiceId = await GetTransactionIdFromTransactionItemId(ItemId);
            if (InvoiceId == 0) return 0;
            var dp = new DynamicParameters();
            dp.Add("@id", ItemId);
            var AffectedCount = await DataAccess.SaveDataAsync(DeleteTransactionItemQuery, dp);
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
            var sqlTemp = $@"SELECT COUNT(t.Id) FROM Transactions t LEFT JOIN (
                            SELECT ti.TransactionId, SUM(ti.Amount * ti.CountValue) AS TotalVal FROM TransactionItems ti WHERE (ti.Amount * ti.CountValue) > 0 GROUP BY ti.TransactionId) AS pos ON t.Id = pos.TransactionId
                            LEFT JOIN (SELECT ti.TransactionId, SUM(ti.Amount * ti.CountValue) AS TotalVal FROM TransactionItems ti WHERE (ti.Amount * ti.CountValue) < 0 GROUP BY ti.TransactionId 
                            ) AS neg ON t.Id = neg.TransactionId
                                { (string.IsNullOrEmpty(WhereClause) ? "" : " WHERE ") } { WhereClause }";
            return await DataAccess.ExecuteScalarAsync<int, DynamicParameters>(sqlTemp, null);
        }

        public async Task<int> GetTotalTransactionItemQueryCountAsync(string WhereClause, int Id)
        {
            var sqlTemp = $@"SELECT COUNT([Id]) FROM TransactionItems WHERE [TransactionId] = { Id }
                                { (string.IsNullOrEmpty(WhereClause) ? "" : " AND ") } { WhereClause }";
            return await DataAccess.ExecuteScalarAsync<int, DynamicParameters>(sqlTemp, null);
        }

        public async Task<ObservableCollection<TransactionListModel>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, string OrderBy = QueryOrderBy, OrderType Order = QueryOrderType)
        {
            string sql = $@"SET NOCOUNT ON
                            SELECT t.Id, t.[FileName], t.DateCreated, t.TimeCreated, t.DateUpdated, t.TimeUpdated, t.Descriptions, ISNULL(pos.TotalVal, 0) AS TotalPositiveItemsSum, ISNULL(neg.TotalVal, 0) AS TotalNegativeItemsSum
                            FROM Transactions t LEFT JOIN (
                            SELECT ti.TransactionId, SUM(ti.Amount * ti.CountValue) AS TotalVal FROM TransactionItems ti WHERE (ti.Amount * ti.CountValue) > 0 GROUP BY ti.TransactionId) AS pos ON t.Id = pos.TransactionId
                            LEFT JOIN (SELECT ti.TransactionId, SUM(ti.Amount * ti.CountValue) AS TotalVal FROM TransactionItems ti WHERE (ti.Amount * ti.CountValue) < 0 GROUP BY ti.TransactionId 
                            ) AS neg ON t.Id = neg.TransactionId

                            { (string.IsNullOrEmpty(WhereClause) ? "" : $" WHERE { WhereClause }") }
                            ORDER BY [{OrderBy}] {Order} OFFSET {OffSet} ROWS FETCH NEXT {FetcheSize} ROWS ONLY";
            return await DataAccess.LoadDataAsync<TransactionListModel, DynamicParameters>(sql, null);
        }

        public async Task<ObservableCollection<TransactionItemModel>> LoadManyTransactionItemsAsync(int OffSet, int FetcheSize, string WhereClause, int Id, string OrderBy = QueryOrderBy, OrderType Order = QueryOrderType)
        {
            string sql = $@"SELECT * FROM TransactionItems WHERE [TransactionId] = { Id }
                            { (string.IsNullOrEmpty(WhereClause) ? "" : $" AND { WhereClause }") }
                            ORDER BY [{OrderBy}] {Order} OFFSET {OffSet} ROWS FETCH NEXT {FetcheSize} ROWS ONLY";
            return await DataAccess.LoadDataAsync<TransactionItemModel, DynamicParameters>(sql, null);
        }

        public async Task<TransactionModel> LoadSingleItemAsync(int Id)
        {
            var query = string.Format(LoadSingleItemQuery, Id);
            var result = await DataAccess.LoadDataAsync<TransactionModel, DynamicParameters>(query, null);
            var output = result.SingleOrDefault();
            if (output != null)
            {
                output.TotalPositiveItemsSum = await LoadTotalPositive(Id);
                output.TotalNegativeItemsSum = await LoadTotalNegative(Id);
            }
            return output;
        }

        public async Task<double> LoadTotalPositive(int Id)
        {
            var sql = $"SELECT ISNULL(SUM([Amount]*[CountValue]), 0) FROM TransactionItems WHERE ([Amount]*[CountValue]) > 0 AND TransactionId = { Id }";
            return await DataAccess.ExecuteScalarAsync<double, DynamicParameters>(sql, null);
        }

        public async Task<double> LoadTotalNegative(int Id)
        {
            var sql = $"SELECT ISNULL(SUM([Amount]*[CountValue]), 0) FROM TransactionItems WHERE ([Amount]*[CountValue]) < 0 AND TransactionId = { Id }";
            return await DataAccess.ExecuteScalarAsync<double, DynamicParameters>(sql, null);
        }
    }
}