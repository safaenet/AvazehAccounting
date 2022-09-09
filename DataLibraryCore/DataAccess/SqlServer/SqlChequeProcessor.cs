using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.ObjectModel;
using FluentValidation.Results;
using DataLibraryCore.DataAccess.Interfaces;
using SharedLibrary.DalModels;
using SharedLibrary.Enums;
using SharedLibrary.Validators;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.SqlServer
{
    public class SqlChequeProcessor<TModel, TSub, TValidator> : IGeneralProcessor<TModel>
        where TModel : ChequeModel where TSub : ChequeEventModel where TValidator : ChequeValidator, new()
    {
        public SqlChequeProcessor(IDataAccess dataAcess)
        {
            DataAccess = dataAcess;
        }

        private readonly IDataAccess DataAccess;
        private const string QueryOrderBy = "DueDate";
        private const OrderType QueryOrderType = OrderType.DESC;
        private readonly string CreateChequeQuery = @"INSERT INTO Cheques (Drawer, Orderer, PayAmount, About, IssueDate, DueDate, BankName, Serial, Identifier, Descriptions)
            VALUES (@drawer, @orderer, @payAmount, @about, @issueDate, @dueDate, @bankName, @serial, @identifier, @descriptions);
            SELECT @id = @@IDENTITY;";
        private readonly string UpdateChequeQuery = @"UPDATE Cheques SET Drawer = @drawer, Orderer = @orderer, PayAmount = @payAmount, About = @about, IssueDate = @issueDate,
            DueDate = @dueDate, BankName = @bankName, Serial = @serial, Identifier = @identifier, Descriptions = @descriptions
            WHERE Id = @id";
        private readonly string InsertEventsQuery = @$"INSERT INTO ChequeEvents (ChequeId, EventDate, EventType, EventText)
                                 VALUES (@ChequeId, @EventDate, @EventType, @EventText)";
        private readonly string SelectChequeQuery = @"SET NOCOUNT ON
            DECLARE @cheques TABLE(
	        [Id] [int],
	        [Drawer] [nvarchar](50),
	        [Orderer] [nvarchar](50),
	        [PayAmount] [bigint],
	        [About] [nvarchar](100),
	        [IssueDate] [char](10),
	        [DueDate] [char](10),
	        [BankName] [nvarchar](50),
	        [Serial] [nvarchar](25),
	        [Identifier] [char](20),
	        [Descriptions] [ntext])
            {0}
            SELECT * FROM @cheques ORDER BY {1} {2};
            SELECT * FROM ChequeEvents WHERE ChequeId IN (SELECT c.Id FROM @cheques c);";
        private readonly string DeleteChequeQuery = @"DELETE FROM Cheques WHERE Id = @id";

        public string GenerateWhereClause(string val, SqlSearchMode mode = SqlSearchMode.OR)
        {
            var criteria = string.IsNullOrWhiteSpace(val) ? "'%'" : $"'%{ val }%'";
            return @$"(CAST([Id] AS varchar) LIKE { criteria }
                      {mode} [Drawer] LIKE { criteria }
                      {mode} [Orderer] LIKE { criteria }
                      {mode} CAST([PayAmount] AS varchar) LIKE { criteria }
                      {mode} [About] LIKE { criteria }
                      {mode} [IssueDate] LIKE { criteria }
                      {mode} [DueDate] LIKE { criteria }
                      {mode} [BankName] LIKE { criteria }
                      {mode} [Serial] LIKE { criteria }
                      {mode} [Identifier] LIKE { criteria }
                      {mode} [Descriptions] LIKE { criteria } )";
        }

        public ValidationResult ValidateItem(TModel cheque)
        {
            TValidator validator = new();
            var result = validator.Validate(cheque);
            return result;
        }

        public async Task<int> CreateItemAsync(TModel item)
        {
            if (item == null || !ValidateItem(item).IsValid) return 0;
            var dp = new DynamicParameters();
            dp.Add("@id", 0, DbType.Int32, ParameterDirection.Output);
            dp.Add("@drawer", item.Drawer);
            dp.Add("@orderer", item.Orderer);
            dp.Add("@payAmount", item.PayAmount);
            dp.Add("@about", item.About);
            dp.Add("@issueDate", item.IssueDate);
            dp.Add("@dueDate", item.DueDate);
            dp.Add("@bankName", item.BankName);
            dp.Add("@serial", item.Serial);
            dp.Add("@identifier", item.Identifier);
            dp.Add("@descriptions", item.Descriptions);
            int AffectedCount = await DataAccess.SaveDataAsync(CreateChequeQuery, dp);
            int OutputId = dp.Get<int>("@id");
            if (AffectedCount > 0)
            {
                item.Id = OutputId;
                await InsertChequeEventsToDatabaseAsync(item).ConfigureAwait(false);
            }
            return OutputId;
        }

        public async Task<int> UpdateItemAsync(TModel item)
        {
            if (item == null || !ValidateItem(item).IsValid) return 0;
            var AffectedCount = await DataAccess.SaveDataAsync(UpdateChequeQuery, item);
            if (AffectedCount > 0)
            {
                string sqlEvents = $"DELETE FROM ChequeEvents WHERE ChequeId = { item.Id }";
                await DataAccess.SaveDataAsync<DynamicParameters>(sqlEvents, null).ConfigureAwait(false);
                await InsertChequeEventsToDatabaseAsync(item).ConfigureAwait(false);
            }
            return AffectedCount;
        }

        private async Task<int> InsertChequeEventsToDatabaseAsync(TModel cheque)
        {
            if (cheque == null || cheque.Events == null || cheque.Events.Count == 0) return 0;
            ObservableCollection<TSub> events = new();
            foreach (var e in cheque.Events)
            {
                if (true)
                {
                    e.ChequeId = cheque.Id;
                    events.Add(e as TSub);
                }
            }
            if (events.Count == 0) return 0;
            return await DataAccess.SaveDataAsync(InsertEventsQuery, events).ConfigureAwait(false);
        }

        public async Task<int> DeleteItemByIdAsync(int Id)
        {
            var dp = new DynamicParameters();
            dp.Add("@id", Id);
            return await DataAccess.SaveDataAsync(DeleteChequeQuery, dp);
        }

        public async Task<int> GetTotalQueryCountAsync(string WhereClause)
        {
            var sqlTemp = $@"SELECT COUNT([Id]) FROM Cheques
                             { (string.IsNullOrEmpty(WhereClause) ? "" : " WHERE ") } { WhereClause }";
            return await DataAccess.ExecuteScalarAsync<int, DynamicParameters>(sqlTemp, null);
        }

        public async Task<ObservableCollection<TModel>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, string OrderBy = QueryOrderBy, OrderType Order = QueryOrderType)
        {
            string sqlTemp = $@"INSERT @cheques SELECT * FROM Cheques
                                { (string.IsNullOrEmpty(WhereClause) ? "" : $" WHERE { WhereClause }") } ORDER BY [{ OrderBy }]
                                OFFSET {OffSet} ROWS FETCH NEXT {FetcheSize} ROWS ONLY";
            string query = string.Format(SelectChequeQuery, sqlTemp, OrderBy, Order);
            using IDbConnection conn = new SqlConnection(DataAccess.GetConnectionString());
            var reader = await conn.QueryMultipleAsync(query, null);
            var Mapped = reader.MapObservableCollectionOfChequesAsync<TModel, TSub, int>
                      (
                         cheque => cheque.Id,
                         e => e.ChequeId,
                         (cheque, phones) => { cheque.Events = new(phones); }
                      );
            return await Mapped;
        }

        public async Task<TModel> LoadSingleItemAsync(int Id)
        {
            var outPut = await LoadManyItemsAsync(0, 1, $"[Id] = { Id }");
            return outPut.FirstOrDefault();
        }
    }
}