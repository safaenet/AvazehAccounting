using AvazehApiClient.DataAccess.Interfaces;
using AvazehApiClient.Models;
using FluentValidation.Results;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AvazehApiClient.DataAccess.Interfaces
{
    public interface ICollectionManager<T>
    {
        IApiProcessor ApiProcessor { get; init; }
        int CurrentPage { get; }
        ObservableCollection<T> Items { get; set; }
        int? MaxID { get; }
        int? MinID { get; }
        int PagesCount { get; }
        int PageSize { get; set; }
        string SearchValue { get; set; }

        event EventHandler PageLoading;
        event EventHandler PageLoaded;
        event EventHandler FirstPageLoaded;
        event EventHandler NextPageLoaded;
        event EventHandler NextPageLoading;
        event EventHandler PreviousPageLoaded;
        event EventHandler PreviousPageLoading;

        Task<T> CreateItemAsync(T item);
        Task<bool> DeleteItemAsync(int Id);
        T GetItemFromCollectionById(int Id);
        Task<T> GetItemById(int Id);
        Task<int> GotoPageAsync(int PageNumber);
        Task<int> LoadFirstPageAsync();
        Task<int> LoadNextPageAsync();
        Task<int> LoadPreviousPageAsync();
        Task<T> UpdateItemAsync(T item);
        ValidationResult ValidateItem(T item);
    }
}