using AvazehWpfApiClient.Models;
using FluentValidation.Results;
using System;
using System.Collections.ObjectModel;

namespace AvazehWpfApiClient.DataAccess.Interfaces
{
    public partial interface IProductCollectionManager
    {
        IProductProcessor Processor { get; init; }
        event EventHandler FirstPageLoaded;
        event EventHandler WhereClauseChanged;
        event EventHandler NextPageLoading;
        event EventHandler NextPageLoaded;
        event EventHandler PreviousPageLoading;
        event EventHandler PreviousPageLoaded;
        int CurrentPage { get; }
        bool Initialized { get; }
        ObservableCollection<ProductModel> Items { get; set; }
        int? MaxID { get; }
        int? MinID { get; }
        int PagesCount { get; }
        int PageSize { get; set; }
        string SearchValue { get; }
        string WhereClause { get; set; }

        bool DeleteItemFromCollectionById(int Id);
        bool DeleteItemFromDbById(int Id);
        int GenerateWhereClause(string val, bool run = false, SqlSearchMode mode = SqlSearchMode.OR);
        ProductModel GetItemFromCollectionById(int Id);
        int GotoPage(int PageNumber);
        int LoadFirstPage();
        int LoadNextPage();
        int LoadPreviousPage();
    }
}