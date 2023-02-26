using SharedLibrary.Enums;

namespace DataLibraryCore.DataAccess.Interfaces;

public interface ITransactionCollectionManagerBase<TModel> : ICollectionManagerBase<TModel>
{
    ITransactionProcessor Processor { get; init; }
    TransactionFinancialStatus? FinStatus { get; }
    int GenerateWhereClause(string val, string OrderBy, OrderType orderType, TransactionFinancialStatus? finStatus, bool run = false, SqlSearchMode mode = SqlSearchMode.OR);
}