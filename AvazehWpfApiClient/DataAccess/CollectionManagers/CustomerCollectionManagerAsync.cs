using AvazehWpfApiClient.DataAccess.Interfaces;
using AvazehWpfApiClient.Models;
using AvazehWpfApiClient.Models.Validators;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvazehWpfApiClient.DataAccess.CollectionManagers
{
    public abstract class CustomerCollectionManagerAsync<T, U, V> : ICollectionManager<T> where T : CustomerModel where U : CustomerModel_DTO_Create_Update where V : CustomerValidator, new()
    {
        public CustomerCollectionManagerAsync(IApiProcessor apiProcessor)
        {
            ApiProcessor = apiProcessor;
        }
        public event EventHandler WhereClauseChanged;
        public event EventHandler PageLoading;
        public event EventHandler PageLoaded;
        public event EventHandler FirstPageLoaded;
        public event EventHandler NextPageLoading;
        public event EventHandler NextPageLoaded;
        public event EventHandler PreviousPageLoading;
        public event EventHandler PreviousPageLoaded;
        private const string Key = "Customer";
        public IApiProcessor ApiProcessor { get; init; }

        public ObservableCollection<T> Items { get; set; }
        public int? MinID => Items == null || Items.Count == 0 ? null : Items.Min(x => x.Id);
        public int? MaxID => Items == null || Items.Count == 0 ? null : Items.Max(x => x.Id);

        public string SearchValue { get; set; }

        public int PageSize { get; set; } = 50;
        public int PagesCount { get; private set; }
        public int CurrentPage { get; private set; }
        public T GetItemFromCollectionById(int Id)
        {
            return Items.SingleOrDefault(i => i.Id == Id);
        }
        public async Task<T> CreateItemAsync(T item)
        {
            if (item == null || !ValidateItem(item).IsValid) return null;
            var newItem = item.AsDto();
            return await ApiProcessor.CreateItemAsync<U, T>(Key, newItem as U);
        }

        public async Task<T> UpdateItemAsync(T item)
        {
            if (item == null || !ValidateItem(item).IsValid) return null;
            var newItem = item.AsDto();
            return await ApiProcessor.UpdateItemAsync<U, T>(Key, item.Id, newItem as U);
        }

        public async Task<bool> DeleteItemAsync(int Id)
        {
            if (await ApiProcessor.DeleteItemAsync(Key, Id))
            {
                Items.Remove(GetItemFromCollectionById(Id));
                return true;
            }
            return false;
        }

        public ValidationResult ValidateItem(T item)
        {
            V validator = new();
            var result = validator.Validate(item);
            return result;
        }

        public async Task<int> GotoPageAsync(int PageNumber)
        {
            PageLoadEventArgs eventArgs = new();
            PageLoading?.Invoke(this, eventArgs);
            if (eventArgs.Cancel) return 0;
            var collection = await ApiProcessor.GetCollectionAsync<ItemsCollection_DTO<T>>(Key, PageNumber, SearchValue, PageSize);
            Items = collection.Items;
            CurrentPage = collection.CurrentPage;
            PagesCount = collection.PagesCount;
            PageLoaded?.Invoke(this, null);
            return Items == null ? 0 : Items.Count;
        }

        public async Task<int> LoadFirstPageAsync()
        {
            var result = await GotoPageAsync(1);
            FirstPageLoaded?.Invoke(this, null);
            return result;
        }

        public async Task<int> LoadPreviousPageAsync()
        {
            PageLoadEventArgs eventArgs = new();
            PreviousPageLoading?.Invoke(this, eventArgs);
            if (eventArgs.Cancel) return 0;
            var result = await GotoPageAsync(CurrentPage - 1);
            PreviousPageLoaded?.Invoke(this, null);
            return result;
        }

        public async Task<int> LoadNextPageAsync()
        {
            PageLoadEventArgs eventArgs = new();
            NextPageLoading?.Invoke(this, eventArgs);
            if (eventArgs.Cancel) return 0;
            var result = await GotoPageAsync(CurrentPage + 1);
            NextPageLoaded?.Invoke(this, null);
            return result;
        }
    }
}