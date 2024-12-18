using DataLibraryCore.DataAccess.Interfaces;
using SharedLibrary.DalModels;
using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.CollectionManagers;

public class TransactionCollectionManager : ITransactionCollectionManager
{
    public TransactionCollectionManager(ITransactionProcessor processor)
    {
        Processor = processor;
    }
    public bool Initialized { get; set; }
    public ITransactionProcessor Processor { get; init; }

    public IEnumerable<TransactionListModel> Items { get; set; }

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


    public TransactionFinancialStatus? FinStatus { get; private set; }

    public int PageSize { get; set; } = 50;
    public int PagesCount => TotalQueryCount == 0 ? 0 : (int)Math.Ceiling((double)TotalQueryCount / PageSize);
    private protected int TotalQueryCount { get; set; }
    public int CurrentPage { get; private set; }

    public int GenerateWhereClause(string val, string OrderBy, OrderType orderType, TransactionFinancialStatus? finStatus, int Id = 0, string Date = null, bool run = false, SqlSearchMode mode = SqlSearchMode.OR)
    {
        if (val == SearchValue && OrderBy == QueryOrderBy && orderType == QueryOrderType) return 0;
        SearchValue = val;
        QueryOrderBy = OrderBy;
        QueryOrderType = orderType;
        WhereClause = Processor.GenerateWhereClause(val, finStatus, Id, Date, mode);
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
        Items = await Processor.LoadManyItemsAsync((PageNumber - 1) * PageSize, PageSize, WhereClause, QueryOrderBy, QueryOrderType);
        CurrentPage = Items == null || Items.Count() == 0 ? 0 : PageNumber;
        return Items == null ? 0 : Items.Count();
    }
}