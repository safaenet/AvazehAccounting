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
        private readonly string CreateTransactionQuery = @"INSERT INTO Transactions (FileName, DateCreated, TimeCreated, Descriptions)
            VALUES (@fileName, @dateCreated, @timeCreated, @descriptions);
            SELECT @id = @@IDENTITY;";
        private readonly string UpdateTransactionQuery = @"UPDATE Transactions SET FileName = @fileName, DateUpdated = @dateUpdated, TimeUpdated = @timeUpdated, Descriptions = @descriptions
            WHERE Id = @id";
        private readonly string DeleteTransactionQuery = @"DELETE FROM Transactions WHERE Id = @id";
        private readonly string GetSingleTransactionItemQuery = "SELECT * FROM TransactionItems WHERE [Id] = {0}";
        private readonly string InsertTransactionItemQuery = @"INSERT INTO TransactionItems (TransactionId, Title, Amount, CountString, CountValue, DateCreated, TimeCreated, Descriptions)
            VALUES (@transactionId, @title, @amount, @countString, @countValue, @dateCreated, @timeCreated, @descriptions); SELECT @id = @@IDENTITY;";
        private readonly string UpdateTransactionItemQuery = @"UPDATE TransactionItems SET Title = @title, Amount = @amount,
            CountString = @countString, CountValue = @countValue, DateUpdated = @dateUpdated, TimeUpdated = @timeUpdated, Descriptions = @descriptions WHERE [Id] = @id";
        private readonly string DeleteTransactionItemQuery = @$"DELETE FROM TransactionItems WHERE [Id] = @id";
        private readonly string UpdateSubItemDateAndTimeQuery = @"UPDATE Transactions SET DateUpdated = @dateUpdated, TimeUpdated = @timeUpdated WHERE [Id] = @id";

        private async Task<int> GetTransactionIdFromTransactionItemId(int Id)
        {
            var query = $"SELECT TransactionId FROM TransactionItems WHERE Id = { Id }";
            return await DataAccess.ExecuteScalarAsync<int, DynamicParameters>(query, null);
        }

        public string GenerateWhereClause(string val, SqlSearchMode mode)
        {
            var criteria = string.IsNullOrWhiteSpace(val) ? "'%'" : $"'%{ val }%'";
            return @$"(CAST([Id] AS varchar) LIKE {criteria}
                      {mode} [FileName] LIKE {criteria}
                      {mode} [DateCreated] LIKE {criteria}
                      {mode} [TimeCreated] LIKE {criteria}
                      {mode} [DateUpdated] LIKE {criteria}
                      {mode} [TimeUpdated] LIKE {criteria}
                      {mode} [Descriptions] LIKE {criteria} )";
        }

        public ValidationResult ValidateItem(TransactionModel product)
        {
            TransactionValidator validator = new();
            var result = validator.Validate(product);
            return result;
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
            var sqlTemp = $@"SELECT COUNT([Id]) FROM Transactions t LEFT JOIN TransactionItems ti ON t.Id = ti.TransactionId
                                { (string.IsNullOrEmpty(WhereClause) ? "" : " WHERE ") } { WhereClause }";
            return await DataAccess.ExecuteScalarAsync<int, DynamicParameters>(sqlTemp, null);
        }

        public async Task<ObservableCollection<TransactionModel>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, string OrderBy = QueryOrderBy, OrderType Order = QueryOrderType)
        {
            string sql = $@"SET NOCOUNT ON
                            SELECT * FROM Transactions
                            { (string.IsNullOrEmpty(WhereClause) ? "" : $" WHERE { WhereClause }") }
                            ORDER BY [{OrderBy}] {Order} OFFSET {OffSet} ROWS FETCH NEXT {FetcheSize} ROWS ONLY";
            return await DataAccess.LoadDataAsync<TransactionModel, DynamicParameters>(sql, null);
        }

        public async Task<TransactionModel> LoadSingleItemAsync(int Id)
        {
            var outPut = await LoadManyItemsAsync(0, 1, $"[Id] = { Id }");
            return outPut.FirstOrDefault();
        }
    }
}