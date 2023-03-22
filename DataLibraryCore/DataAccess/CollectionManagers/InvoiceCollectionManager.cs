﻿using DataLibraryCore.DataAccess.Interfaces;
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
    public event EventHandler WhereClauseChanged;
    public event EventHandler FirstPageLoaded;
    public event EventHandler NextPageLoading;
    public event EventHandler NextPageLoaded;
    public event EventHandler PreviousPageLoading;
    public event EventHandler PreviousPageLoaded;
    public bool Initialized { get; set; }
    public IInvoiceProcessor Processor { get; init; }
    public IEnumerable<InvoiceListModel> Items { get; set; }
    public int? MinID => Items == null || Items.Count() == 0 ? null : Items.Min(x => x.Id);
    public int? MaxID => Items == null || Items.Count() == 0 ? null : Items.Max(x => x.Id);

    private protected string _WhereClause;
    public string WhereClause
    {
        get => _WhereClause;
        set
        {
            if (_WhereClause != value)
                Initialized = false;
            _WhereClause = value;
            WhereClauseChanged?.Invoke(this, null);
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
        if (run) LoadFirstPageAsync().ConfigureAwait(true);
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
        Items = await Processor.LoadManyItemsAsync(PageSize, -1, -1, "%", "%", InvoiceLifeStatus.Active, InvoiceFinancialStatus.Deptor, SqlQueryOrderMode.DESC, -1);
        CurrentPage = Items == null || Items.Count() == 0 ? 0 : PageNumber;
        return Items == null ? 0 : Items.Count();
    }

    public async Task<int> LoadFirstPageAsync()
    {
        var result = await GotoPageAsync(1);
        FirstPageLoaded?.Invoke(this, null);
        return result;
    }

    public async Task<int> LoadPreviousPageAsync()
    {
        PageLoadEventArgs eventArgs = new();
        PreviousPageLoading?.Invoke(this, eventArgs);
        if (eventArgs.Cancel) return 0;
        var result = await GotoPageAsync(CurrentPage - 1);
        PreviousPageLoaded?.Invoke(this, null);
        return result;
    }

    public async Task<int> LoadNextPageAsync()
    {
        PageLoadEventArgs eventArgs = new();
        NextPageLoading?.Invoke(this, eventArgs);
        if (eventArgs.Cancel) return 0;
        var result = await GotoPageAsync(CurrentPage + 1);
        NextPageLoaded?.Invoke(this, null);
        return result;
    }
}