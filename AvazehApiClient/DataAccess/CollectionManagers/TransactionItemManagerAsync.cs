using FluentValidation.Results;
using System.Threading.Tasks;
using AvazehApiClient.DataAccess.Interfaces;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Validators;
using System.Collections.ObjectModel;
using System;
using SharedLibrary.Enums;
using System.Linq;

namespace AvazehApiClient.DataAccess.CollectionManagers
{
    public class TransactionDetailManager : ITransactionDetailManager
    {
        public TransactionDetailManager(IApiProcessor apiProcessor)
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
        private readonly string Key = "TransactionItem";
        private string KeyWithId => Key + "/" + TransactionId.ToString();
        public IApiProcessor ApiProcessor { get; init; }
        public int TransactionId { get; set; }
        public ObservableCollection<TransactionItemModel> Items { get; set; }
        public int? MinID => Items == null || Items.Count == 0 ? null : Items.Min(x => x.Id);
        public int? MaxID => Items == null || Items.Count == 0 ? null : Items.Max(x => x.Id);

        public string SearchValue { get; set; }
        public string QueryOrderBy { get; set; } = "Id";
        public OrderType QueryOrderType { get; set; } = OrderType.DESC;
        public TransactionFinancialStatus? FinStatus { get; set; }

        public int PageSize { get; set; } = 100;
        public int PagesCount { get; private set; }
        public int CurrentPage { get; private set; }
        public TransactionItemModel GetItemFromCollectionById(int Id)
        {
            return Items.SingleOrDefault(i => i.Id == Id);
        }

        public async Task<TransactionItemModel> GetItemById(int Id)
        {
            return await ApiProcessor.GetItemAsync<TransactionItemModel>(Key, Id.ToString());
        }

        public async Task<TransactionItemModel> CreateItemAsync(TransactionItemModel item)
        {
            if (item == null || !ValidateItem(item).IsValid) return null;
            var newItem = item.AsDto();
            return await ApiProcessor.CreateItemAsync<TransactionItemModel_DTO_Create_Update, TransactionItemModel>(Key, newItem);
        }

        public async Task<TransactionItemModel> UpdateItemAsync(TransactionItemModel item)
        {
            if (item == null || !ValidateItem(item).IsValid) return null;
            var newItem = item.AsDto();
            var result = await ApiProcessor.UpdateItemAsync<TransactionItemModel_DTO_Create_Update, TransactionItemModel>(Key, item.Id, newItem);
            result.DateCreated = item.DateCreated;
            result.TimeCreated = item.TimeCreated;
            return result;
        }

        public async Task<bool> DeleteItemAsync(int Id)
        {
            if (await ApiProcessor.DeleteItemAsync(Key, Id))
                return true;
            return false;
        }

        public ValidationResult ValidateItem(TransactionItemModel item)
        {
            TransactionItemValidator validator = new();
            var result = validator.Validate(item);
            return result;
        }

        public async Task<int> GotoPageAsync(int PageNumber, bool Refresh = false)
        {
            PageLoadEventArgs eventArgs = new();
            PageLoading?.Invoke(this, eventArgs);
            if (eventArgs.Cancel) return 0;
            var collection = await ApiProcessor.GetTransactionCollectionAsync<ItemsCollection_DTO<TransactionItemModel>>(KeyWithId, QueryOrderBy, QueryOrderType, PageNumber, SearchValue, FinStatus, PageSize, Refresh);
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
    }
}