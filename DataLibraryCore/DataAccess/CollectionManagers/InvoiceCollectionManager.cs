using DataLibraryCore.DataAccess.Interfaces;
using DataLibraryCore.DataAccess.SqlServer;
using DataLibraryCore.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace DataLibraryCore.DataAccess.CollectionManagers
{
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
        public bool Initialized { get; private set; }
        public IInvoiceProcessor Processor { get; init; }
        public ObservableCollection<InvoiceListModel> Invoices { get; set; }
        public int? MinID => Invoices == null || Invoices.Count == 0 ? null : Invoices.Min(x => x.Id);
        public int? MaxID => Invoices == null || Invoices.Count == 0 ? null : Invoices.Max(x => x.Id);
        public double TotalBalance => Invoices.Sum(x => x.TotalBalance);
        public double TotalPayments => Invoices.Sum(x => x.TotalPayments);

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
                if (TotalQueryCount == 0) return 0;
                Initialized = true;
            }
            PageNumber = PageNumber < 1 ? 1 : PageNumber;
            PageNumber = PageNumber > PagesCount ? PagesCount : PageNumber;
            Invoices = Processor.LoadManyItems((PageNumber - 1) * PageSize, PageSize, WhereClause);
            CurrentPage = Invoices == null || Invoices.Count == 0 ? 0 : PageNumber;
            return Invoices == null ? 0 : Invoices.Count;
        }

        public InvoiceListModel GetItemFromCollectionById(int Id)
        {
            return Invoices.SingleOrDefault(i => i.Id == Id);
        }
        public bool DeleteItemFromCollectionById(int Id)
        {
            return Invoices.Remove(GetItemFromCollectionById(Id));
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

        public int GenerateWhereClause(string val, InvoiceLifeStatus? LifeStatus, InvoiceFinancialStatus? FinStatus, SqlSearchMode mode = SqlSearchMode.OR)
        {
            if (val == SearchValue) return 0;
            SearchValue = val;
            WhereClause = Processor.GenerateWhereClause(val, LifeStatus, FinStatus, mode);
            return Invoices == null ? 0 : Invoices.Count;
        }
    }
}