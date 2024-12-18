﻿using DataLibraryCore.DataAccess.Interfaces;
using SharedLibrary.DalModels;
using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.CollectionManagers;

public class ChequeCollectionManager : IChequeCollectionManager
{
    public ChequeCollectionManager(IChequeProcessor processor)
    {
        Processor = processor;
    }
    public bool Initialized { get; set; }
    public IChequeProcessor Processor { get; init; }
    public IEnumerable<ChequeModel> Items { get; set; }

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

    public int PageSize { get; set; } = 50;
    public int PagesCount => TotalQueryCount == 0 ? 0 : (int)Math.Ceiling((double)TotalQueryCount / PageSize);
    private protected int TotalQueryCount { get; set; }
    public int CurrentPage { get; private set; }

    public string SearchValue { get; private set; }
    private protected string _QueryOrderBy;
    private protected OrderType _OrderType;
    private protected ChequeListQueryStatus? listQueryStatus;

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

    public ChequeListQueryStatus? ListQueryStatus
    {
        get => listQueryStatus;
        private set
        {
            if (ListQueryStatus != value)
                Initialized = false;
            listQueryStatus = value;
        }
    }

    public int GenerateWhereClause(string val, string OrderBy, OrderType orderType, ChequeListQueryStatus? listQueryStatus = ChequeListQueryStatus.FromNowOn, bool run = false, SqlSearchMode mode = SqlSearchMode.OR)
    {
        if (val == SearchValue && OrderBy == QueryOrderBy && orderType == QueryOrderType && listQueryStatus == ListQueryStatus) return 0;
        SearchValue = val;
        QueryOrderBy = OrderBy;
        QueryOrderType = orderType;
        ListQueryStatus = listQueryStatus;
        WhereClause = Processor.GenerateWhereClause(val, listQueryStatus, mode);
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