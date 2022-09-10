﻿using Dapper;
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
using System.Globalization;
using DataLibraryCore.Models;

namespace DataLibraryCore.DataAccess.SqlServer
{
    public class SqlChequeProcessor : IChequeProcessor
        //where TModel : ChequeModel where TSub : ChequeEventModel where TValidator : ChequeValidator, new()
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
        private readonly string DeleteChequeQuery = @"DELETE FROM Cheques WHERE [Id] = @id";

        public string GenerateWhereClause(string val, ChequeListQueryStatus? listQueryStatus, SqlSearchMode mode = SqlSearchMode.OR)
        {
            var criteria = string.IsNullOrWhiteSpace(val) ? "'%'" : $"'%{ val }%'";
            var persianDate = PersianCalendarModel.GetCurrentPersianDate();
            //NotCashed : WHERE [EventType] != 2 AND [EventType] != 4
            //Cashed : WHERE [EventType] = 4
            //Sold : WHERE [EventType] = 2
            //NonSufficientFund : WHERE [EventType] = 3
            //FromNowOn : WHERE ([EventType] != 4 AND [EventType] !=2) OR ([EventType] = 2 AND c.[DueDate] <= '{CurrentDate}')
            var queryStatusCriteria = listQueryStatus switch
            {
                ChequeListQueryStatus.NotCashed => " AND (ISNULL(ce1.[EventType], 0) != 2 AND ISNULL(ce1.[EventType], 0) != 4)",
                ChequeListQueryStatus.Cashed => " AND ISNULL(ce1.[EventType], 0) = 4",
                ChequeListQueryStatus.Sold => " AND ISNULL(ce1.[EventType], 0) = 2",
                ChequeListQueryStatus.NonSufficientFund => " AND ISNULL(ce1.[EventType], 0) = 3",
                ChequeListQueryStatus.FromNowOn => $" AND ((ISNULL(ce1.[EventType], 0) != 4 AND ISNULL(ce1.[EventType], 0) !=2) OR (ISNULL(ce1.[EventType], 0) = 2 AND c.[DueDate] >= '{ persianDate }'))",
                _ => ""
            };
            return @$"(CAST(c.[Id] AS varchar) LIKE { criteria }
                      {mode} c.[Drawer] LIKE { criteria }
                      {mode} c.[Orderer] LIKE { criteria }
                      {mode} CAST(c.[PayAmount] AS varchar) LIKE { criteria }
                      {mode} c.[About] LIKE { criteria }
                      {mode} c.[IssueDate] LIKE { criteria }
                      {mode} c.[DueDate] LIKE { criteria }
                      {mode} c.[BankName] LIKE { criteria }
                      {mode} c.[Serial] LIKE { criteria }
                      {mode} c.[Identifier] LIKE { criteria }
                      {mode} c.[Descriptions] LIKE { criteria } ) { queryStatusCriteria }";
        }

        public ValidationResult ValidateItem(ChequeModel cheque)
        {
            ChequeValidator validator = new();
            var result = validator.Validate(cheque);
            return result;
        }

        public async Task<int> CreateItemAsync(ChequeModel item)
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

        public async Task<int> UpdateItemAsync(ChequeModel item)
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

        private async Task<int> InsertChequeEventsToDatabaseAsync(ChequeModel cheque)
        {
            if (cheque == null || cheque.Events == null || cheque.Events.Count == 0) return 0;
            ObservableCollection<ChequeEventModel> events = new();
            foreach (var e in cheque.Events)
            {
                if (true)
                {
                    e.ChequeId = cheque.Id;
                    events.Add(e as ChequeEventModel);
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
            var sqlTemp = $@"SELECT COUNT(c.Id) FROM Cheques c LEFT JOIN ChequeEvents ce1 ON (c.Id = ce1.ChequeId) LEFT OUTER JOIN ChequeEvents ce2 ON (c.Id = ce2.ChequeId AND (ce1.Id < ce2.Id)) WHERE ce2.Id IS NULL
                             { (string.IsNullOrEmpty(WhereClause) ? "" : " AND ") } { WhereClause }";
            return await DataAccess.ExecuteScalarAsync<int, DynamicParameters>(sqlTemp, null);
        }

        public async Task<ObservableCollection<ChequeModel>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, string OrderBy = QueryOrderBy, OrderType Order = QueryOrderType)
        {
            string sqlTemp = $@"INSERT @cheques SELECT c.* FROM Cheques c LEFT JOIN ChequeEvents ce1 ON (c.Id = ce1.ChequeId) LEFT OUTER JOIN ChequeEvents ce2 ON (c.Id = ce2.ChequeId AND (ce1.Id < ce2.Id)) WHERE ce2.Id IS NULL
                                { (string.IsNullOrEmpty(WhereClause) ? "" : $" AND { WhereClause }") } ORDER BY [{ OrderBy }]
                                OFFSET {OffSet} ROWS FETCH NEXT {FetcheSize} ROWS ONLY";
            string query = string.Format(SelectChequeQuery, sqlTemp, OrderBy, Order);
            using IDbConnection conn = new SqlConnection(DataAccess.GetConnectionString());
            var reader = await conn.QueryMultipleAsync(query, null);
            var Mapped = reader.MapObservableCollectionOfChequesAsync<ChequeModel, ChequeEventModel, int>
                      (
                         cheque => cheque.Id,
                         e => e.ChequeId,
                         (cheque, phones) => { cheque.Events = new(phones); }
                      );
            return await Mapped;
        }

        public async Task<ChequeModel> LoadSingleItemAsync(int Id)
        {
            var outPut = await LoadManyItemsAsync(0, 1, $"[Id] = { Id }");
            return outPut.FirstOrDefault();
        }
    }
}