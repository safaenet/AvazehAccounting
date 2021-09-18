using DataLibraryCore.DataAccess.Interfaces;
using DataLibraryCore.Models;
using FluentValidation.Results;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace DataLibraryCore.DataAccess.CollectionManagers
{
    public class ChequeCollectionManager : IChequeCollectionManager
    {
        public ChequeCollectionManager(IChequeProcessor processor)
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
        public IChequeProcessor Processor { get; init; }
        public ObservableCollection<ChequeModel> Items { get; set; }
        public int? MinID => Items == null || Items.Count == 0 ? null : Items.Min(x => x.Id);
        public int? MaxID => Items == null || Items.Count == 0 ? null : Items.Max(x => x.Id);
        public long TotalChequeAmount => Items == null || Items.Count == 0 ? 0 : Items.Sum(x => x.PayAmount);

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
            if (Processor.DeleteItemByID(Id) > 0)
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
                _WhereClause = value;
                WhereClauseChanged?.Invoke(this, null);
                Initialized = false;
            }
        }

        public int PageSize { get; set; } = 50;
        public int PagesCount => TotalQueryCount == 0 ? 0 : (int)Math.Ceiling((double)TotalQueryCount / PageSize);
        private protected int TotalQueryCount { get; set; }
        public int CurrentPage { get; private set; }

        public string SearchValue { get; private set; }

        public int GotoPage(int PageNumber)
        {
            if (!Initialized)
            {
                TotalQueryCount = Processor.GetTotalQueryCount(WhereClause);
                if (TotalQueryCount == 0) return 0;
                Initialized = true;
            }
            PageNumber = PageNumber < 1 ? 1 : PageNumber;
            PageNumber = PageNumber > PagesCount ? PagesCount : PageNumber;
            Items = Processor.LoadManyItems((PageNumber - 1) * PageSize, PageSize, WhereClause);
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

        public int GenerateWhereClause(string val, SqlSearchMode mode = SqlSearchMode.OR)
        {
            SearchValue = val;
            WhereClause = Processor.GenerateWhereClause(val, mode);
            return Items == null ? 0 : Items.Count;
        }
    }
}