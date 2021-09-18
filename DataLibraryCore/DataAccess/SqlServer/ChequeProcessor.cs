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

namespace DataLibraryCore.DataAccess.SqlServer
{
    public class SqlChequeProcessor : Interfaces.IChequeProcessor
    {
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
            SELECT * FROM @cheques ORDER BY [Id] ASC;
            SELECT * FROM ChequeEvents WHERE ChequeId IN (SELECT c.Id FROM @cheques c);";
        private readonly string DeleteChequeQuery = @"DELETE FROM Cheques WHERE Id = @id";

        public int CreateItem(ChequeModel cheque)
        {
            if (cheque == null) return 0;
            var dp = new DynamicParameters();
            dp.Add("@id", 0, DbType.Int32, ParameterDirection.Output);
            dp.Add("@drawer", cheque.Drawer);
            dp.Add("@orderer", cheque.Orderer);
            dp.Add("@payAmount", cheque.PayAmount);
            dp.Add("@about", cheque.About);
            dp.Add("@issueDate", cheque.IssueDate);
            dp.Add("@dueDate", cheque.DueDate);
            dp.Add("@bankName", cheque.BankName);
            dp.Add("@serial", cheque.Serial);
            dp.Add("@identifier", cheque.Identifier);
            dp.Add("@descriptions", cheque.Descriptions);
            int AffectedCount = DataAccess.SaveData(CreateChequeQuery, dp);
            int OutputId = dp.Get<int>("@id");
            if (AffectedCount > 0)
            {
                cheque.Id = OutputId;
                InsertChequeEventsToDatabase(cheque);
            }
            return OutputId;
        }

        public int UpdateItem(ChequeModel cheque)
        {
            if (cheque == null) return 0;
            var AffectedCount = DataAccess.SaveData(UpdateChequeQuery, cheque);
            if (AffectedCount > 0)
            {
                string sqlEvents = $"DELETE FROM ChequeEvents WHERE ChequeId = { cheque.Id }";
                DataAccess.SaveData<DynamicParameters>(sqlEvents, null);
                InsertChequeEventsToDatabase(cheque);
            }
            return AffectedCount;
        }

        private int InsertChequeEventsToDatabase(ChequeModel cheque)
        {
            if (cheque == null || cheque.Events == null || cheque.Events.Count == 0) return 0;
            ObservableCollection<ChequeEventModel> events = new();
            foreach (var e in cheque.Events)
            {
                if (true)
                {
                    e.ChequeId = cheque.Id;
                    events.Add(e);
                }
            }
            if (events.Count == 0) return 0;
            return DataAccess.SaveData(InsertEventsQuery, events);
        }

        public int DeleteItemByID(int ID)
        {
            var dp = new DynamicParameters();
            dp.Add("@id", ID);
            return DataAccess.SaveData(DeleteChequeQuery, dp);
        }

        public int GetTotalQueryCount(string WhereClause)
        {
            var sqlTemp = $@"SELECT COUNT([Id]) FROM Cheques
                             { (string.IsNullOrEmpty(WhereClause) ? "" : " WHERE ") } { WhereClause }";
            return DataAccess.ExecuteScalar<int, DynamicParameters>(sqlTemp, null);
        }

        public ObservableCollection<ChequeModel> LoadManyItems(int OffSet, int FetcheSize, string WhereClause, OrderType Order = OrderType.DESC, string OrderBy = "DueDate")
        {
            string sqlTemp = $@"INSERT @cheques SELECT * FROM Cheques
                                { (string.IsNullOrEmpty(WhereClause) ? "" : $" WHERE { WhereClause }") }
                                ORDER BY [{OrderBy}] {Order} OFFSET {OffSet} ROWS FETCH NEXT {FetcheSize} ROWS ONLY";
            string query = string.Format(SelectChequeQuery, sqlTemp);
            using IDbConnection conn = new SqlConnection(DataAccess.GetConnectionString());
            var reader = conn.QueryMultipleAsync(query, null).Result;
            var Mapped = reader.MapObservableCollectionOfCheques<ChequeModel, ChequeEventModel, int>
                      (
                         cheque => cheque.Id,
                         e => e.ChequeId,
                         (cheque, phones) => { cheque.Events = new(phones); }
                      );
            return Mapped;
        }

        public ChequeModel LoadSingleItem(int ID)
        {
            return LoadManyItems(0, 1, $"[Id] = { ID }").FirstOrDefault();
        }

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

        public ValidationResult ValidateItem(ChequeModel cheque)
        {
            ChequeValidator validator = new();
            var result = validator.Validate(cheque);
            return result;
        }
    }
}