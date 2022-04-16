using SharedLibrary.DalModels;
using SharedLibrary.Enums;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public interface ITransactionCollectionManager2<TModel> : ICollectionManagerBase<TModel>
    {
        ITransactionProcessor Processor { get; init; }
        TransactionFinancialStatus? FinStatus { get; }
        int GenerateWhereClause(string val, string OrderBy, OrderType orderType, TransactionFinancialStatus? finStatus, bool run = false, SqlSearchMode mode = SqlSearchMode.OR);
    }
}