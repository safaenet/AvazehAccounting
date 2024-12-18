using DataLibraryCore.DataAccess.Interfaces;
using SharedLibrary.DalModels;
using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.CollectionManagers;

public class InvoiceCollectionManager : IInvoiceCollectionManager
{
    public InvoiceCollectionManager(IInvoiceProcessor processor)
    {
        Processor = processor;
    }
    public bool Initialized { get; set; }
    public IInvoiceProcessor Processor { get; init; }
    public IEnumerable<InvoiceListModel> Items { get; set; }

    private protected string _WhereClause;
    public string WhereClause
    {
        get => _WhereClause;
        set
        {
            if (_WhereClause != value)
                Initialized = false;
            _WhereClause = value;
        }
    }

    public string SearchValue { get; private set; }
    private protected string _QueryOrderBy;
    private protected OrderType _OrderType;

    public string QueryOrderBy
    {
        get => _QueryOrderBy;
        private set
        {
            if (QueryOrderBy != value)
                Initialized = false;
            _QueryOrderBy = value;
        }
    }

    public OrderType QueryOrderType
    {
        get => _OrderType;
        private set
        {
            if (_OrderType != value)
                Initialized = false;
            _OrderType = value;
        }
    }
    public InvoiceLifeStatus? LifeStatus { get; private set; }
    public InvoiceFinancialStatus? FinStatus { get; private set; }
    public int PageSize { get; set; } = 50;
    public int PagesCount => TotalQueryCount == 0 ? 0 : (int)Math.Ceiling((double)TotalQueryCount / PageSize);
    private protected int TotalQueryCount { get; set; }
    public int CurrentPage { get; private set; }

    public int GenerateWhereClause(string val, string OrderBy, OrderType orderType, InvoiceLifeStatus? lifeStatus, InvoiceFinancialStatus? finStatus, bool run = false, SqlSearchMode mode = SqlSearchMode.OR)
    {
        if (val == SearchValue && OrderBy == QueryOrderBy && orderType == QueryOrderType && LifeStatus == lifeStatus && FinStatus == finStatus) return 0;
        SearchValue = val;
        QueryOrderBy = OrderBy;
        QueryOrderType = orderType;
        LifeStatus = lifeStatus;
        FinStatus = finStatus;
        WhereClause = Processor.GenerateWhereClause(val, lifeStatus, finStatus, mode);
        if (run) GotoPageAsync(1).ConfigureAwait(true);
        return Items == null ? 0 : Items.Count();
    }

    public async Task<int> GotoPageAsync(int PageNumber)
    {
        if (!Initialized)
        {
            TotalQueryCount = await Processor.GetTotalQueryCountAsync(WhereClause);
            if (TotalQueryCount == 0)
            {
                Items = null;
                return 0;
            }
            Initialized = true;
        }
        if (PagesCount == 0) PageNumber = 1;
        else if (PageNumber > PagesCount) PageNumber = PagesCount;
        else if (PageNumber < 1) PageNumber = 1;
        //Items = await Processor.LoadManyItemsAsync((PageNumber - 1) * PageSize, PageSize, WhereClause, QueryOrderBy, QueryOrderType);
        Items = await Processor.LoadManyItemsAsync(PageSize, -1, -1, "%", "%", InvoiceLifeStatus.Active, InvoiceFinancialStatus.Deptor, SqlQuerySearchMode.Backward, OrderType.DESC, -1);
        CurrentPage = Items == null || !Items.Any() ? 0 : PageNumber;
        return Items == null ? 0 : Items.Count();
    }
}