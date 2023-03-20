using SharedLibrary.DalModels;
using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces;

public interface IChequeProcessorBase<TModel> : IProcessorBase<TModel>
{
    string GenerateWhereClause(string val, ChequeListQueryStatus? listQueryStatus, SqlSearchMode mode);
    Task<List<TModel>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, string OrderBy, OrderType Order);
    Task<List<string>> GetBanknames();
    Task<List<ChequeModel>> LoadChequesByDueDate(string FromDate, string ToDate);
}