using AvazehApiClient.DataAccess.Interfaces;
using FluentValidation.Results;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using SharedLibrary.Validators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace AvazehApiClient.DataAccess.CollectionManagers;

public class InvoiceCollectionManagerAsync : IInvoiceCollectionManager
{
    public InvoiceCollectionManagerAsync(IApiProcessor apiProcessor)
    {
        ApiProcessor = apiProcessor;
    }
    public event EventHandler PageLoading;
    public event EventHandler PageLoaded;
    public event EventHandler FirstPageLoaded;
    public event EventHandler NextPageLoading;
    public event EventHandler NextPageLoaded;
    public event EventHandler PreviousPageLoading;
    public event EventHandler PreviousPageLoaded;
    private const string Key = "Invoices";
    public IApiProcessor ApiProcessor { get; init; }

    public ObservableCollection<InvoiceListModel> Items { get; set; }

    public int PageSize { get; set; } = 50;
    public int InvoiceIdToSearch { get; set; }
    public int CustomerIdToSearch { get; set; }
    public string InvoiceDate { get; set; }
    public string SearchValue { get; set; }
    public InvoiceLifeStatus? LifeStatus { get; set; }
    public InvoiceFinancialStatus? FinStatus { get; set; }
    public OrderType orderType { get; set; }

    public InvoiceListModel GetItemFromCollectionById(int Id)
    {
        return Items.SingleOrDefault(i => i.Id == Id);
    }

    public async Task<InvoiceModel> GetItemById(int Id)
    {
        return await ApiProcessor.GetItemAsync<InvoiceModel>(Key, Id.ToString());
    }

    public async Task<InvoiceModel> CreateItemAsync(InvoiceModel item)
    {
        if (item == null || !ValidateItem(item).IsValid) return null;
        var newItem = item.AsDto();
        return await ApiProcessor.CreateItemAsync<InvoiceModel_DTO_Create_Update, InvoiceModel>(Key, newItem);
    }

    public async Task<InvoiceModel> UpdateItemAsync(InvoiceModel item)
    {
        if (item == null || !ValidateItem(item).IsValid) return null;
        var newItem = item.AsDto();
        return await ApiProcessor.UpdateItemAsync<InvoiceModel_DTO_Create_Update, InvoiceModel>(Key, item.Id, newItem);
    }

    public async Task<bool> DeleteItemAsync(int Id)
    {
        if (await ApiProcessor.DeleteItemAsync(Key, Id))
        {
            var item = GetItemFromCollectionById(Id);
            if (item != null)
                Items.Remove(item);
            return true;
        }
        return false;
    }

    public ValidationResult ValidateItem(InvoiceModel item)
    {
        InvoiceValidator validator = new();
        var result = validator.Validate(item);
        return result;
    }

    public async Task<int> LoadItemsAsync(SqlQuerySearchMode SearchMode, int StartId)
    {
        var collection = await ApiProcessor.GetInvoiceCollectionAsync<List<InvoiceListModel>>(Key, PageSize, InvoiceIdToSearch, CustomerIdToSearch, InvoiceDate, SearchValue, LifeStatus, FinStatus, SearchMode, orderType, StartId);
        Items = collection?.AsObservable();
        return collection == null ? 0 : collection.Count;
    }

    public async Task<int> RefreshPage()
    {
        return await LoadFirstPageAsync(); //To be improved.
    }

    public async Task<int> LoadFirstPageAsync()
    {
        var result = await LoadItemsAsync(SqlQuerySearchMode.Backward, -1);
        FirstPageLoaded?.Invoke(this, null);
        return result;
    }

    public async Task<int> LoadPreviousPageAsync()
    {
        if (Items == null || Items.Count == 0) return await LoadFirstPageAsync();
        PageLoadEventArgs eventArgs = new();
        PreviousPageLoading?.Invoke(this, eventArgs);
        if (eventArgs.Cancel) return 0;
        var result = await LoadItemsAsync(SqlQuerySearchMode.Backward, Items.Min(x => x.Id));
        PreviousPageLoaded?.Invoke(this, null);
        return result;
    }

    public async Task<int> LoadNextPageAsync()
    {
        if (Items == null || Items.Count == 0) return await LoadFirstPageAsync();
        PageLoadEventArgs eventArgs = new();
        NextPageLoading?.Invoke(this, eventArgs);
        if (eventArgs.Cancel) return 0;
        var result = await LoadItemsAsync(SqlQuerySearchMode.Forward, Items.Max(x => x.Id));
        NextPageLoaded?.Invoke(this, null);
        return result;
    }

    public async Task<List<ItemsForComboBox>> LoadProductItems(string SearchText = null)
    {
        var collection = await ApiProcessor.GetCollectionAsync<List<ItemsForComboBox>>(Key + "/ProductItems", SearchText);
        return collection is null ? null : collection;
    }

    public async Task<double> GetCustomerTotalBalanceById(int CustomerId, int InvoiceId = 0)
    {
        return (double)await ApiProcessor.GetValueOrNullAsync<double>(Key + "/CustomerBalance", CustomerId, InvoiceId);
    }

    public async Task<List<UserDescriptionModel>> GetUserDescriptions()
    {
        var collection = await ApiProcessor.GetCollectionAsync<List<UserDescriptionModel>>(Key + "/UserDescriptions", null);
        return collection is null ? null : collection;
    }
}