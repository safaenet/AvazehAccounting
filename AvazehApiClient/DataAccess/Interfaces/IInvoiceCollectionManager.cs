﻿using FluentValidation.Results;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AvazehApiClient.DataAccess.Interfaces;

public interface IInvoiceCollectionManager
{
    IApiProcessor ApiProcessor { get; init; }
    SqlQuerySearchMode SearchMode { get; set; }
    public int PageSize { get; set; }
    public int InvoiceIdToSearch { get; set; }
    public int CustomerIdToSearch { get; set; }
    public string InvoiceDateToSearch { get; set; }
    public string SearchValue { get; set; }
    public InvoiceLifeStatus? LifeStatus { get; set; }
    public InvoiceFinancialStatus? FinStatus { get; set; }
    public OrderType orderType { get; set; }

    event EventHandler PageLoading;
    event EventHandler PageLoaded;
    event EventHandler FirstPageLoaded;
    event EventHandler NextPageLoaded;
    event EventHandler NextPageLoading;
    event EventHandler PreviousPageLoaded;
    event EventHandler PreviousPageLoading;

    Task<InvoiceModel> CreateItemAsync(InvoiceModel item);
    Task<InvoiceModel> UpdateItemAsync(InvoiceModel item);
    Task<bool> DeleteItemAsync(int Id);
    Task<int> LoadItemsAsync(SqlQuerySearchMode SearchMode, int StartId);
    Task<int> RefreshPage();
    Task<int> LoadFirstPageAsync();
    Task<int> LoadNextPageAsync();
    Task<int> LoadPreviousPageAsync();
    Task<InvoiceModel> GetItemById(int Id);
    ValidationResult ValidateItem(InvoiceModel item);

    ObservableCollection<InvoiceListModel> Items { get; set; }
    Task<bool> SetPrevInvoiceId(int InvoiceId, int PrevInvoiceId);
    Task<ObservableCollection<InvoiceListModel>> LoadPrevInvoices(int InvoiceId, string InvoiceDate, string searchValue, OrderType orderType);
    InvoiceListModel GetItemFromCollectionById(int Id);
    Task<List<ItemsForComboBox>> LoadProductItems(string SearchText = null);
    Task<decimal> GetCustomerTotalBalanceById(int CustomerId, int InvoiceId = 0);
    Task<decimal> GetInvoicePrevTotalBalanceById(int InvoiceId);
    Task<List<UserDescriptionModel>> GetUserDescriptions();
}