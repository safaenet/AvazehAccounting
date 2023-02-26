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

public class TransactionCollectionManagerAsync : ITransactionCollectionManager
{
    public TransactionCollectionManagerAsync(IApiProcessor apiProcessor)
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
    private const string Key = "Transactions";
    public IApiProcessor ApiProcessor { get; init; }

    public ObservableCollection<TransactionListModel> Items { get; set; }
    public int? MinID => Items == null || Items.Count == 0 ? null : Items.Min(x => x.Id);
    public int? MaxID => Items == null || Items.Count == 0 ? null : Items.Max(x => x.Id);

    public string SearchValue { get; set; }
    public string QueryOrderBy { get; set; } = "Id";
    public OrderType QueryOrderType { get; set; } = OrderType.DESC;
    public TransactionFinancialStatus? FinStatus { get; set; }

    public int PageSize { get; set; } = 50;
    public int PagesCount { get; private set; }
    public int CurrentPage { get; private set; }
    public TransactionListModel GetItemFromCollectionById(int Id)
    {
        return Items.SingleOrDefault(i => i.Id == Id);
    }

    public async Task<TransactionModel> GetItemById(int Id)
    {
        return await ApiProcessor.GetItemAsync<TransactionModel>(Key, Id.ToString());
    }

    public async Task<TransactionModel> CreateItemAsync(TransactionModel item)
    {
        if (item == null || !ValidateItem(item).IsValid) return null;
        var newItem = item.AsDto();
        return await ApiProcessor.CreateItemAsync<TransactionModel_DTO_Create_Update, TransactionModel>(Key, newItem);
    }

    public async Task<TransactionModel> UpdateItemAsync(TransactionModel item)
    {
        if (item == null || !ValidateItem(item).IsValid) return null;
        var newItem = item.AsDto();
        return await ApiProcessor.UpdateItemAsync<TransactionModel_DTO_Create_Update, TransactionModel>(Key, item.Id, newItem);
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

    public ValidationResult ValidateItem(TransactionModel item)
    {
        TransactionValidator validator = new();
        var result = validator.Validate(item);
        return result;
    }

    public async Task<int> GotoPageAsync(int PageNumber, bool Refresh = false)
    {
        PageLoadEventArgs eventArgs = new();
        PageLoading?.Invoke(this, eventArgs);
        if (eventArgs.Cancel) return 0;
        var collection = await ApiProcessor.GetTransactionCollectionAsync<ItemsCollection_DTO<TransactionListModel>>(Key, QueryOrderBy, QueryOrderType, PageNumber, SearchValue, FinStatus, PageSize, Refresh);
        Items = collection?.Items;
        CurrentPage = collection is null ? 0 : collection.CurrentPage;
        PagesCount = collection is null ? 0 : collection.PagesCount;
        PageLoaded?.Invoke(this, null);
        return Items == null ? 0 : Items.Count;
    }

    public async Task<int> RefreshPage()
    {
        return await GotoPageAsync(CurrentPage, true);
    }

    public async Task<int> LoadFirstPageAsync()
    {
        var result = await GotoPageAsync(1);
        FirstPageLoaded?.Invoke(this, null);
        return result;
    }

    public async Task<int> LoadPreviousPageAsync()
    {
        if (CurrentPage == 1) return 0;
        PageLoadEventArgs eventArgs = new();
        PreviousPageLoading?.Invoke(this, eventArgs);
        if (eventArgs.Cancel) return 0;
        var result = await GotoPageAsync(CurrentPage - 1);
        PreviousPageLoaded?.Invoke(this, null);
        return result;
    }

    public async Task<int> LoadNextPageAsync()
    {
        if (CurrentPage == PagesCount) return 0;
        PageLoadEventArgs eventArgs = new();
        NextPageLoading?.Invoke(this, eventArgs);
        if (eventArgs.Cancel) return 0;
        var result = await GotoPageAsync(CurrentPage + 1);
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
}