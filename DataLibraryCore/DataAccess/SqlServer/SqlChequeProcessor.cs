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
using System.Collections.Generic;
using SharedLibrary.Helpers;
using System;
using Serilog;

namespace DataLibraryCore.DataAccess.SqlServer;

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
    private readonly string CreateChequeQuery = @"DECLARE @newId int; SET @newId = (SELECT ISNULL(MAX([Id]), 0) FROM [Cheques]) + 1;
            INSERT INTO Cheques ([Id], Drawer, Orderer, PayAmount, About, IssueDate, DueDate, BankName, Serial, Identifier, Descriptions)
            VALUES (@newId, @drawer, @orderer, @payAmount, @about, @issueDate, @dueDate, @bankName, @serial, @identifier, @descriptions);
            SELECT @id = @newId;";
    private readonly string UpdateChequeQuery = @"UPDATE Cheques SET Drawer = @drawer, Orderer = @orderer, PayAmount = @payAmount, About = @about, IssueDate = @issueDate,
            DueDate = @dueDate, BankName = @bankName, Serial = @serial, Identifier = @identifier, Descriptions = @descriptions
            WHERE Id = @id";
    private readonly string InsertEventsQuery = @$"DECLARE @newId int; SET @newId = (SELECT ISNULL(MAX([Id]), 0) FROM [ChequeEvents]) + 1;
            INSERT INTO ChequeEvents ([Id], ChequeId, EventDate, EventType, EventText)
            VALUES (@newId, @ChequeId, @EventDate, @EventType, @EventText)";
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
    private readonly string LoadBanknamesQuery = "SELECT DISTINCT [BankName] FROM [Cheques]";

    public string GenerateWhereClause(string val, ChequeListQueryStatus? listQueryStatus, SqlSearchMode mode = SqlSearchMode.OR)
    {
        try
        {
            var criteria = string.IsNullOrWhiteSpace(val) ? "'%'" : $"'%{ val }%'";
            var persianDate = PersianCalendarHelper.GetCurrentPersianDate();
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
                      {mode} c.[Drawer] LIKE N{ criteria }
                      {mode} c.[Orderer] LIKE N{ criteria }
                      {mode} CAST(c.[PayAmount] AS varchar) LIKE { criteria }
                      {mode} c.[About] LIKE N{ criteria }
                      {mode} c.[IssueDate] LIKE { criteria }
                      {mode} c.[DueDate] LIKE { criteria }
                      {mode} c.[BankName] LIKE N{ criteria }
                      {mode} c.[Serial] LIKE { criteria }
                      {mode} c.[Identifier] LIKE { criteria }
                      {mode} c.[Descriptions] LIKE N{ criteria } ) { queryStatusCriteria }";
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in IChequeProcessor");
        }
        return null;
    }

    public ValidationResult ValidateItem(ChequeModel cheque)
    {
        try
        {
            ChequeValidator validator = new();
            var result = validator.Validate(cheque);
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in IChequeProcessor");
        }
        return null;
    }

    public async Task<int> CreateItemAsync(ChequeModel item)
    {
        try
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
        catch (Exception ex)
        {
            Log.Error(ex, "Error in IChequeProcessor");
        }
        return 0;
    }

    public async Task<int> UpdateItemAsync(ChequeModel item)
    {
        try
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
        catch (Exception ex)
        {
            Log.Error(ex, "Error in IChequeProcessor");
        }
        return 0;
    }

    private async Task<int> InsertChequeEventsToDatabaseAsync(ChequeModel cheque)
    {
        try
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
        catch (Exception ex)
        {
            Log.Error(ex, "Error in IChequeProcessor");
        }
        return 0;
    }

    public async Task<int> DeleteItemByIdAsync(int Id)
    {
        try
        {
            var dp = new DynamicParameters();
            dp.Add("@id", Id);
            return await DataAccess.SaveDataAsync(DeleteChequeQuery, dp);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in IChequeProcessor");
        }
        return 0;
    }

    public async Task<List<string>> GetBanknames()
    {
        try
        {
            var result = await DataAccess.LoadDataAsync<string, DynamicParameters>(LoadBanknamesQuery, null);
            return result.ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in IChequeProcessor");
        }
        return null;
    }

    public async Task<ObservableCollection<ChequeModel>> LoadChequesByDueDate(string FromDate, string ToDate)
    {
        try
        {
            string LoadCloseChequesQuery = $" CAST(REPLACE(c.DueDate,'/','') AS bigint) <= CAST('{ ToDate }' AS bigint) AND CAST(REPLACE(c.DueDate,'/','') AS bigint) >= CAST('{ FromDate }' AS bigint) ";
            var result = await LoadManyItemsAsync(0, int.MaxValue, LoadCloseChequesQuery);
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in IChequeProcessor");
        }
        return null;
    }

    public async Task<int> GetTotalQueryCountAsync(string WhereClause)
    {
        try
        {
            var sqlTemp = $@"SELECT COUNT(c.Id) FROM Cheques c LEFT JOIN ChequeEvents ce1 ON (c.Id = ce1.ChequeId) LEFT OUTER JOIN ChequeEvents ce2 ON (c.Id = ce2.ChequeId AND (ce1.Id < ce2.Id)) WHERE ce2.Id IS NULL
                             { (string.IsNullOrEmpty(WhereClause) ? "" : " AND ") } { WhereClause }";
            return await DataAccess.ExecuteScalarAsync<int, DynamicParameters>(sqlTemp, null);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in IChequeProcessor");
        }
        return 0;
    }

    public async Task<ObservableCollection<ChequeModel>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, string OrderBy = QueryOrderBy, OrderType Order = QueryOrderType)
    {
        try
        {
            string sqlTemp = $@"INSERT @cheques SELECT c.* FROM Cheques c LEFT JOIN ChequeEvents ce1 ON (c.Id = ce1.ChequeId) LEFT OUTER JOIN ChequeEvents ce2 ON (c.Id = ce2.ChequeId AND (ce1.Id < ce2.Id)) WHERE ce2.Id IS NULL
                                { (string.IsNullOrEmpty(WhereClause) ? "" : $" AND { WhereClause }") } 
                                ORDER BY [{ OrderBy }] OFFSET {OffSet} ROWS FETCH NEXT {FetcheSize} ROWS ONLY";
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
        catch (Exception ex)
        {
            Log.Error(ex, "Error in IChequeProcessor");
        }
        return null;
    }

    public async Task<ChequeModel> LoadSingleItemAsync(int Id)
    {
        try
        {
            var outPut = await LoadManyItemsAsync(0, 1, $"[Id] = { Id }");
            return outPut.FirstOrDefault();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in IChequeProcessor");
        }
        return null;
    }
}