using FluentValidation.Results;
using SharedLibrary.Enums;
using System;
using System.Threading.Tasks;

namespace AvazehApiClient.DataAccess.Interfaces;

public interface ICollectionManagerBase<T>
{
    IApiProcessor ApiProcessor { get; init; }
    int CurrentPage { get; }
    int PagesCount { get; }
    int PageSize { get; set; }
    string SearchValue { get; set; }
    string QueryOrderBy { get; set; }
    OrderType QueryOrderType { get; set; }

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