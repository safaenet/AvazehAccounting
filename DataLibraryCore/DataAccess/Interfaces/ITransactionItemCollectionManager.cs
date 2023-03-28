using SharedLibrary.DalModels;
using SharedLibrary.Enums;

namespace DataLibraryCore.DataAccess.Interfaces;

public interface ITransactionItemCollectionManager : ICollectionManagerBase<TransactionItemModel>
{
    ITransactionProcessor Processor { get; init; }
    TransactionFinancialStatus? FinStatus { get; }
    int TransactionId { get; set; }
    int GenerateWhereClause(string val, string OrderBy, OrderType orderType, TransactionFinancialStatus? finStatus, string Date, bool run = false, SqlSearchMode mode = SqlSearchMode.OR);
}