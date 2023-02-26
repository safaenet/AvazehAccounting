using SharedLibrary.Enums;

namespace DataLibraryCore.DataAccess.Interfaces;

public interface IInvoiceCollectionManagerBase<TModel> : ICollectionManagerBase<TModel>
{
    IInvoiceProcessor Processor { get; init; }
    InvoiceLifeStatus? LifeStatus { get; }
    InvoiceFinancialStatus? FinStatus { get; }
    int GenerateWhereClause(string val, string OrderBy, OrderType orderType, InvoiceLifeStatus? lifeStatus, InvoiceFinancialStatus? finStatus, bool run = false, SqlSearchMode mode = SqlSearchMode.OR);
}