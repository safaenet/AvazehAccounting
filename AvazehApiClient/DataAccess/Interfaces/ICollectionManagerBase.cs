using FluentValidation.Results;
using SharedLibrary.Enums;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AvazehApiClient.DataAccess.Interfaces
{
    public interface ICollectionManagerBase<T>
    {
        IApiProcessor ApiProcessor { get; init; }
        int CurrentPage { get; }
        int? MaxID { get; }
        int? MinID { get; }
        int PagesCount { get; }
        int PageSize { get; set; }
        string SearchValue { get; set; }
        string QueryOrderBy { get; set; }
        OrderType QueryOrderType { get; set; }

        event EventHandler PageLoading;
        event EventHandler PageLoaded;
        event EventHandler FirstPageLoaded;
        event EventHandler NextPageLoaded;
        event EventHandler NextPageLoading;
        event EventHandler PreviousPageLoaded;
        event EventHandler PreviousPageLoading;

        Task<T> CreateItemAsync(T item);
        Task<T> UpdateItemAsync(T item);
        Task<bool> DeleteItemAsync(int Id);
        Task<int> GotoPageAsync(int PageNumber, bool Refresh);
        Task<int> RefreshPage();
        Task<int> LoadFirstPageAsync();
        Task<int> LoadNextPageAsync();
        Task<int> LoadPreviousPageAsync();
        Task<T> GetItemById(int Id);
        ValidationResult ValidateItem(T item);
    }
}