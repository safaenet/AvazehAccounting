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
    public partial class ProductCollectionManager : ICollectionManager<ProductModel>
    {
        public ProductCollectionManager(IApiProcessor apiProcessor)
        {
            ApiProcessor = apiProcessor;
        }
        public event EventHandler WhereClauseChanged;
        public event EventHandler FirstPageLoaded;
        public event EventHandler NextPageLoading;
        public event EventHandler NextPageLoaded;
        public event EventHandler PreviousPageLoading;
        public event EventHandler PreviousPageLoaded;
        private const string Key = "Product";
        public IApiProcessor ApiProcessor { get; init; }

        public ObservableCollection<ProductModel> Items { get; set; }
        public int? MinID => Items == null || Items.Count == 0 ? null : Items.Min(x => x.Id);
        public int? MaxID => Items == null || Items.Count == 0 ? null : Items.Max(x => x.Id);

        public string SearchValue { get; set; }

        public int PageSize { get; set; } = 50;
        public int PagesCount { get; private set; }
        public int CurrentPage { get; private set; }
        public ProductModel GetItemFromCollectionById(int Id)
        {
            return Items.SingleOrDefault(i => i.Id == Id);
        }
        public async Task<ProductModel> CreateItemAsync(ProductModel item)
        {
            if (item == null || !ValidateItem(item).IsValid) return null;
            var newItem = item.AsDto();
            return await ApiProcessor.CreateItemAsync<ProductModel_DTO_Create_Update, ProductModel>(Key, newItem);
        }

        public async Task<ProductModel> UpdateItemAsync(ProductModel item)
        {
            if (item == null || !ValidateItem(item).IsValid) return null;
            var newItem = item.AsDto();
            return await ApiProcessor.UpdateItemAsync<ProductModel_DTO_Create_Update, ProductModel>(Key, item.Id, newItem);
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

        public ValidationResult ValidateItem(ProductModel product)
        {
            ProductValidator validator = new();
            var result = validator.Validate(product);
            return result;
        }

        public async Task<int> GotoPageAsync(int PageNumber)
        {
            var collection = await ApiProcessor.GetCollectionAsync<ProductItemsCollection_DTO>(Key, PageNumber, SearchValue, PageSize);
            Items = collection.Items;
            CurrentPage = collection.CurrentPage;
            PagesCount = collection.PagesCount;
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