using FluentValidation.Results;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
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
        InvoiceLifeStatus? LifeStatus { get; set; }
        InvoiceFinancialStatus? FinStatus { get; set; }

        event EventHandler FirstPageLoaded;
        event EventHandler NextPageLoaded;
        event EventHandler NextPageLoading;
        event EventHandler PageLoaded;
        event EventHandler PageLoading;
        event EventHandler PreviousPageLoaded;
        event EventHandler PreviousPageLoading;

        Task<InvoiceModel> CreateItemAsync(InvoiceModel item);
        Task<bool> DeleteItemAsync(int Id);
        Task<InvoiceModel> GetItemById(int Id);
        InvoiceListModel GetItemFromCollectionById(int Id);
        Task<int> GotoPageAsync(int PageNumber, bool Refresh);
        Task<int> RefreshPage();
        Task<int> LoadFirstPageAsync();
        Task<int> LoadNextPageAsync();
        Task<int> LoadPreviousPageAsync();
        Task<InvoiceModel> UpdateItemAsync(InvoiceModel item);
        ValidationResult ValidateItem(InvoiceModel item);
        Task<List<ProductNamesForComboBox>> LoadProductItems(string SearchText = null);
        Task<double> GetCustomerTotalBalanceById(int CustomerId, int InvoiceId = 0);
    }
}