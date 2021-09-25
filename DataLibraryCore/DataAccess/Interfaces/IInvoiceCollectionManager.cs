using DataLibraryCore.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public interface IInvoiceCollectionManager
    {
        IInvoiceProcessor Processor { get; init; }
        event EventHandler FirstPageLoaded;
        event EventHandler WhereClauseChanged;
        event EventHandler NextPageLoading;
        event EventHandler NextPageLoaded;
        event EventHandler PreviousPageLoading;
        event EventHandler PreviousPageLoaded;
        int CurrentPage { get; }
        bool Initialized { get; }
        ObservableCollection<InvoiceListModel> Items { get; set; }
        int? MaxID { get; }
        int? MinID { get; }
        int PagesCount { get; }
        int PageSize { get; set; }
        string SearchValue { get; }
        InvoiceLifeStatus? LifeStatus { get; }
        InvoiceFinancialStatus? FinStatus { get; }
        string WhereClause { get; set; }
        bool DeleteItemFromCollectionById(int Id);
        bool DeleteItemFromDbById(int Id);
        int GenerateWhereClause(string val, InvoiceLifeStatus? LifeStatus, InvoiceFinancialStatus? FinStatus, bool run = false, SqlSearchMode mode = SqlSearchMode.OR);
        InvoiceListModel GetItemFromCollectionById(int Id);
        int GotoPage(int PageNumber);
        int LoadFirstPage();
        int LoadNextPage();
        int LoadPreviousPage();

        Task<bool> DeleteItemFromDbByIdAsync(int Id);
        Task<int> GotoPageAsync(int PageNumber);
        Task<int> LoadFirstPageAsync();
        Task<int> LoadNextPageAsync();
        Task<int> LoadPreviousPageAsync();
    }
}