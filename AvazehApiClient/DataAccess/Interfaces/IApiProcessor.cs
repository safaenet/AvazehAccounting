﻿using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using System.Threading.Tasks;

namespace AvazehApiClient.DataAccess.Interfaces;

public interface IApiProcessor
{
    string Token { get; set; }
    Task<bool> TestDBConnectionAsync();
    bool IsInRole(string role);
    string GetRoleValue(string roleType);
    Task<T> GetCollectionAsync<T>(string Key, string OrderBy, OrderType orderType, int Page = 1, string SearchText = "", int PageSize = 50, bool ForceLoad = false) where T : class;
    Task<T> GetCollectionAsync<T>(string Key, string SearchText) where T : class;
    Task<T> GetCollectionAsync<T>(string Key, int Id1, int Id2, int Id3) where T : class;
    Task<T> GetCollectionAsync<T>(string Key, int Id) where T : class;

    Task<bool> GetBooleanAsync(string Key);
    Task<T> GetInvoiceCollectionAsync<T>(string Key, int FetcheSize = 50, int InvoiceId = -1, int CustomerId = -1, string InvoiceDate = "%", string SearchValue = "%", InvoiceLifeStatus? LifeStatus = InvoiceLifeStatus.Active, InvoiceFinancialStatus? FinStatus = InvoiceFinancialStatus.Outstanding, SqlQuerySearchMode SearchMode = SqlQuerySearchMode.Backward, OrderType orderType = OrderType.DESC, int StartId = -1) where T : class;
    Task<T> GetTransactionCollectionAsync<T>(string Key, string OrderBy, OrderType orderType, int IdToSearch = 0, string DateToSearch = null, int Page = 1, string SearchText = "", TransactionFinancialStatus? FinStatus = null, int PageSize = 50, bool ForceLoad = false) where T : class;
    Task<ItemsCollection_DTO<ChequeModel>> GetChequeCollectionAsync(string Key, string OrderBy, OrderType orderType, ChequeListQueryStatus? listQueryStatus = ChequeListQueryStatus.FromNowOn, int Page = 1, string SearchText = "", int PageSize = 50, bool ForceLoad = false);
    Task<T> GetItemAsync<T>(string Key, string Id) where T : class;
    Task<U> CreateItemAsync<T, U>(string Key, T model) where U : class;
    Task<U> UpdateItemAsync<T, U>(string Key, int Id, T model) where U : class;
    Task<bool> DeleteItemAsync(string Key, int Id);
    Task<T?> GetValueOrNullAsync<T>(string Key, params int[] Ids) where T : struct;
    Task<decimal> GetValueOrZeroAsync<T>(string Key, params int[] Ids);
}