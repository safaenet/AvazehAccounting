using DataLibraryCore.DataAccess.Interfaces;
using SharedLibrary.DalModels;
using SharedLibrary.Enums;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace DataLibraryCore.DataAccess.CollectionManagers
{
    public partial class ChequeCollectionManager : ICollectionManager<ChequeModel, IProcessor<ChequeModel>>
    {
        public ChequeCollectionManager(IProcessor<ChequeModel> processor)
        {
            Processor = processor;
        }
        public event EventHandler WhereClauseChanged;
        public event EventHandler FirstPageLoaded;
        public event EventHandler NextPageLoading;
        public event EventHandler NextPageLoaded;
        public event EventHandler PreviousPageLoading;
        public event EventHandler PreviousPageLoaded;
        public bool Initialized { get; private set; }
        public IProcessor<ChequeModel> Processor { get; init; }
        public ObservableCollection<ChequeModel> Items { get; set; }
        public int? MinID => Items == null || Items.Count == 0 ? null : Items.Min(x => x.Id);
        public int? MaxID => Items == null || Items.Count == 0 ? null : Items.Max(x => x.Id);

        public ChequeModel GetItemFromCollectionById(int Id)
        {
            return Items.SingleOrDefault(i => i.Id == Id);
        }
        public bool DeleteItemFromCollectionById(int Id)
        {
            return Items.Remove(GetItemFromCollectionById(Id));
        }

        public bool DeleteItemFromDbById(int Id)
        {
            if (Processor.DeleteItemById(Id) > 0)
            {
                DeleteItemFromCollectionById(Id);
                return true;
            }
            return false;
        }

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

        public int PageSize { get; set; } = 50;
        public int PagesCount => TotalQueryCount == 0 ? 0 : (int)Math.Ceiling((double)TotalQueryCount / PageSize);
        private protected int TotalQueryCount { get; set; }
        public int CurrentPage { get; private set; }

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

        public int GotoPage(int PageNumber)
        {
            if (!Initialized)
            {
                TotalQueryCount = Processor.GetTotalQueryCount(WhereClause);
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
            Items = Processor.LoadManyItems((PageNumber - 1) * PageSize, PageSize, WhereClause, QueryOrderBy, QueryOrderType);
            CurrentPage = Items == null || Items.Count == 0 ? 0 : PageNumber;
            return Items == null ? 0 : Items.Count;
        }
        public int LoadFirstPage()
        {
            var result = GotoPage(1);
            FirstPageLoaded?.Invoke(this, null);
            return result;
        }

        public int LoadPreviousPage()
        {
            PageLoadEventArgs eventArgs = new();
            PreviousPageLoading?.Invoke(this, eventArgs);
            if (eventArgs.Cancel) return 0;
            var result = GotoPage(CurrentPage - 1);
            PreviousPageLoaded?.Invoke(this, null);
            return result;
        }

        public int LoadNextPage()
        {
            PageLoadEventArgs eventArgs = new();
            NextPageLoading?.Invoke(this, eventArgs);
            if (eventArgs.Cancel) return 0;
            var result = GotoPage(CurrentPage + 1);
            NextPageLoaded?.Invoke(this, null);
            return result;
        }

        public int GenerateWhereClause(string val, string OrderBy, OrderType orderType, bool run = false, SqlSearchMode mode = SqlSearchMode.OR)
        {
            if (val == SearchValue && OrderBy == QueryOrderBy && orderType == QueryOrderType) return 0;
            SearchValue = val;
            QueryOrderBy = OrderBy;
            QueryOrderType = orderType;
            WhereClause = Processor.GenerateWhereClause(val, mode);
            if (run) LoadFirstPage();
            return Items == null ? 0 : Items.Count;
        }
    }
}