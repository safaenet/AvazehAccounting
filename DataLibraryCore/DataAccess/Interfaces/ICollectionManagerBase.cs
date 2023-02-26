using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces;

public interface ICollectionManagerBase<TModel>
{
    event EventHandler FirstPageLoaded;
    event EventHandler WhereClauseChanged;
    event EventHandler NextPageLoading;
    event EventHandler NextPageLoaded;
    event EventHandler PreviousPageLoading;
    event EventHandler PreviousPageLoaded;
    int CurrentPage { get; }
    bool Initialized { get; set; }
    ObservableCollection<TModel> Items { get; set; }
    int? MaxID { get; }
    int? MinID { get; }
    int PagesCount { get; }
    int PageSize { get; set; }
    string SearchValue { get; }
    string WhereClause { get; set; }
    Task<int> GotoPageAsync(int PageNumber);
    Task<int> LoadFirstPageAsync();
    Task<int> LoadNextPageAsync();
    Task<int> LoadPreviousPageAsync();
}