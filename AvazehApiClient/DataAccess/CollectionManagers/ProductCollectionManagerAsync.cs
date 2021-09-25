﻿using AvazehApiClient.DataAccess.Interfaces;
using AvazehApiClient.Models;
using AvazehApiClient.Models.Validators;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvazehApiClient.DataAccess.CollectionManagers
{
    public class ProductCollectionManagerAsync<TDalModel, TDtoModel, TValidator> : ICollectionManager<TDalModel> where TDalModel : ProductModel where TDtoModel : ProductModel_DTO_Create_Update where TValidator : ProductValidator, new()
    {
        public ProductCollectionManagerAsync(IApiProcessor apiProcessor)
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
        private const string Key = "Product";
        public IApiProcessor ApiProcessor { get; init; }

        public ObservableCollection<TDalModel> Items { get; set; }
        public int? MinID => Items == null || Items.Count == 0 ? null : Items.Min(x => x.Id);
        public int? MaxID => Items == null || Items.Count == 0 ? null : Items.Max(x => x.Id);

        public string SearchValue { get; set; }

        public int PageSize { get; set; } = 50;
        public int PagesCount { get; private set; }
        public int CurrentPage { get; private set; }
        public TDalModel GetItemFromCollectionById(int Id)
        {
            return Items.SingleOrDefault(i => i.Id == Id);
        }

        public async Task<TDalModel> GetItemById(int Id)
        {
            return await ApiProcessor.GetItemAsync<TDalModel>(Key, Id);
        }

        public async Task<TDalModel> CreateItemAsync(TDalModel item)
        {
            if (item == null || !ValidateItem(item).IsValid) return null;
            var newItem = item.AsDto();
            return await ApiProcessor.CreateItemAsync<TDtoModel, TDalModel>(Key, newItem as TDtoModel);
        }

        public async Task<TDalModel> UpdateItemAsync(TDalModel item)
        {
            if (item == null || !ValidateItem(item).IsValid) return null;
            var newItem = item.AsDto();
            return await ApiProcessor.UpdateItemAsync<TDtoModel, TDalModel>(Key, item.Id, newItem as TDtoModel);
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

        public ValidationResult ValidateItem(TDalModel item)
        {
            TValidator validator = new();
            var result = validator.Validate(item);
            return result;
        }

        public async Task<int> GotoPageAsync(int PageNumber)
        {
            PageLoadEventArgs eventArgs = new();
            PageLoading?.Invoke(this, eventArgs);
            if (eventArgs.Cancel) return 0;
            var collection = await ApiProcessor.GetCollectionAsync<ItemsCollection_DTO<TDalModel>>(Key, PageNumber, SearchValue, PageSize);
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