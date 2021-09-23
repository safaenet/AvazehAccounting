using AvazehWpfApiClient.DataAccess.Interfaces;
using AvazehWpfApiClient.DataAccess.SqlServer;
using AvazehWpfApiClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace AvazehWpfApiClient.DataAccess.CollectionManagers
{
    public partial class InvoiceCollectionManager : IInvoiceCollectionManager
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
        public bool Initialized { get; private set; }
        public IInvoiceProcessor Processor { get; init; }
        public ObservableCollection<InvoiceListModel> Items { get; set; }
        public int? MinID => Items == null || Items.Count == 0 ? null : Items.Min(x => x.Id);
        public int? MaxID => Items == null || Items.Count == 0 ? null : Items.Max(x => x.Id);
        public double TotalBalance => Items.Sum(x => x.TotalBalance);
        public double TotalPayments => Items.Sum(x => x.TotalPayments);

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

        public string SearchValue { get; private set; }
        public int PageSize { get; set; } = 50;
        public int PagesCount => TotalQueryCount == 0 ? 0 : (int)Math.Ceiling((double)TotalQueryCount / PageSize);
        private protected int TotalQueryCount { get; set; }
        public int CurrentPage { get; private set; }

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
            Items = Processor.LoadManyItems((PageNumber - 1) * PageSize, PageSize, WhereClause);
            CurrentPage = Items == null || Items.Count == 0 ? 0 : PageNumber;
            return Items == null ? 0 : Items.Count;
        }

        public InvoiceListModel GetItemFromCollectionById(int Id)
        {
            return Items.SingleOrDefault(i => i.Id == Id);
        }
        public bool DeleteItemFromCollectionById(int Id)
        {
            return Items.Remove(GetItemFromCollectionById(Id));
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

        public bool DeleteItemFromDbById(int Id)
        {
            if (Processor.DeleteItemById(Id) > 0)
            {
                DeleteItemFromCollectionById(Id);
                return true;
            }
            return false;
        }

        public int GenerateWhereClause(string val, InvoiceLifeStatus? LifeStatus, InvoiceFinancialStatus? FinStatus, bool run = false, SqlSearchMode mode = SqlSearchMode.OR)
        {
            if (val == SearchValue) return 0;
            SearchValue = val;
            WhereClause = Processor.GenerateWhereClause(val, LifeStatus, FinStatus, mode);
            if (run) LoadFirstPage();
            return Items == null ? 0 : Items.Count;
        }
    }
}