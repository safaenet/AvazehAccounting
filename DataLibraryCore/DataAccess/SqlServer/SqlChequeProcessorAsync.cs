using Dapper;
using DataLibraryCore.DataAccess.Interfaces;
using DataLibraryCore.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.SqlServer
{
    public partial class SqlChequeProcessor : IChequeProcessor
    {
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
                    events.Add(e);
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

        public async Task<ObservableCollection<ChequeModel>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, OrderType Order = OrderType.DESC, string OrderBy = "DueDate")
        {
            string sqlTemp = $@"INSERT @cheques SELECT * FROM Cheques
                                { (string.IsNullOrEmpty(WhereClause) ? "" : $" WHERE { WhereClause }") }
                                ORDER BY [{OrderBy}] {Order} OFFSET {OffSet} ROWS FETCH NEXT {FetcheSize} ROWS ONLY";
            string query = string.Format(SelectChequeQuery, sqlTemp);
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