using SharedLibrary.DalModels;
using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public interface ITransactionItemCollectionManager : ICollectionManagerBase<TransactionItemModel>
    {
        ITransactionProcessor Processor { get; init; }
        TransactionFinancialStatus? FinStatus { get; }
        int TransactionId { get; set; }
        int GenerateWhereClause(string val, string OrderBy, OrderType orderType, TransactionFinancialStatus? finStatus, bool run = false, SqlSearchMode mode = SqlSearchMode.OR);
    }
}