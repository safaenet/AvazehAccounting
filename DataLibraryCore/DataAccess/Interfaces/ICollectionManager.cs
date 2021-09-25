using DataLibraryCore.Models;
using FluentValidation.Results;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public interface ICollectionManager<TModel, TProcessor>
    {
        TProcessor Processor { get; init; }
        event EventHandler FirstPageLoaded;
        event EventHandler WhereClauseChanged;
        event EventHandler NextPageLoading;
        event EventHandler NextPageLoaded;
        event EventHandler PreviousPageLoading;
        event EventHandler PreviousPageLoaded;
        int CurrentPage { get; }
        bool Initialized { get; }
        ObservableCollection<TModel> Items { get; set; }
        int? MaxID { get; }
        int? MinID { get; }
        int PagesCount { get; }
        int PageSize { get; set; }
        string SearchValue { get; }
        string WhereClause { get; set; }

        bool DeleteItemFromCollectionById(int Id);
        bool DeleteItemFromDbById(int Id);
        int GenerateWhereClause(string val, bool run = false, SqlSearchMode mode = SqlSearchMode.OR);
        TModel GetItemFromCollectionById(int Id);
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