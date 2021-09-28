using AvazehApiClient.DataAccess.Interfaces;
using AvazehApiClient.Models;
using FluentValidation.Results;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AvazehApiClient.DataAccess.Interfaces
{
    public interface IInvoiceCollectionManager
    {
        IApiProcessor ApiProcessor { get; init; }
        int CurrentPage { get; }
        ObservableCollection<InvoiceListModel> Items { get; set; }
        int? MaxID { get; }
        int? MinID { get; }
        int PagesCount { get; }
        int PageSize { get; set; }
        string SearchValue { get; set; }
        string QueryOrderBy { get; set; }
        OrderType QueryOrderType { get; set; }

        event EventHandler FirstPageLoaded;
        event EventHandler NextPageLoaded;
        event EventHandler NextPageLoading;
        event EventHandler PageLoaded;
        event EventHandler PageLoading;
        event EventHandler PreviousPageLoaded;
        event EventHandler PreviousPageLoading;

        Task<InvoiceModel> CreateItemAsync(InvoiceModel item);
        Task<bool> DeleteItemAsync(int Id);
        Task<InvoiceModel_DTO_Read> GetItemById(int Id);
        InvoiceListModel GetItemFromCollectionById(int Id);
        Task<int> GotoPageAsync(int PageNumber);
        Task<int> LoadFirstPageAsync();
        Task<int> LoadNextPageAsync();
        Task<int> LoadPreviousPageAsync();
        Task<InvoiceModel> UpdateItemAsync(InvoiceModel item);
        ValidationResult ValidateItem(InvoiceModel item);
    }
}