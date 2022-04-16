using SharedLibrary.DalModels;
using SharedLibrary.Enums;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public interface IInvoiceCollectionManager2<TModel> : ICollectionManagerBase<TModel>
    {
        IInvoiceProcessor Processor { get; init; }
        InvoiceLifeStatus? LifeStatus { get; }
        InvoiceFinancialStatus? FinStatus { get; }
        int GenerateWhereClause(string val, string OrderBy, OrderType orderType, InvoiceLifeStatus? lifeStatus, InvoiceFinancialStatus? finStatus, bool run = false, SqlSearchMode mode = SqlSearchMode.OR);
    }
}