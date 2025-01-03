﻿using FluentValidation.Results;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces;

public interface IInvoiceProcessorBase<TModel> : IProcessorBase<TModel>
{
    string GenerateWhereClause(string val, InvoiceLifeStatus? LifeStatus, InvoiceFinancialStatus? FinStatus, SqlSearchMode mode = SqlSearchMode.OR);
    ValidationResult ValidateItem(InvoiceItemModel item);
    ValidationResult ValidateItem(InvoicePaymentModel item);
    Task<int> DeleteInvoiceItemFromDatabaseAsync(int ItemId);
    Task<IEnumerable<ItemsForComboBox>> GetProductItemsAsync(string SearchText);
    Task<decimal> GetTotalOrRestTotalBalanceOfCustomerAsync(int CustomerId, int InvoiceId = 0);
    Task<decimal> GetPrevBalanceOfInvoiceAsync(int InvoiceId);
    Task<InvoiceItemModel> GetInvoiceItemFromDatabaseAsync(int Id);
    Task<InvoicePaymentModel> GetInvoicePaymentFromDatabaseAsync(int Id);
    Task<int> InsertInvoiceItemToDatabaseAsync(InvoiceItemModel item);
    Task<IEnumerable<ProductUnitModel>> GetProductUnitsAsync();
    Task<IEnumerable<ItemsForComboBox>> GetCustomerNamesAsync(string SearchText);
    Task<IEnumerable<ItemsForComboBox>> GetInvoiceAboutsAsync(string SearchText);
    Task<IEnumerable<InvoiceListModel>> LoadManyItemsAsync(int FetcheSize, int InvoiceId, int CustomerId, string InvoiceDate, string SearchValue, InvoiceLifeStatus? LifeStatus, InvoiceFinancialStatus? FinStatus, SqlQuerySearchMode SearchMode, OrderType orderType, int StartId);
    Task<int> UpdateInvoiceItemInDatabaseAsync(InvoiceItemModel item);
    Task<int> SetPrevInvoiceId(int InvoiceId, int PrevInvoiceId);
    Task<List<InvoiceListModel>> GetPrevInvoices(int InvoiceId, int CustomerId, string InvoiceDate, string SearchValue, OrderType orderType);
    Task<int> InsertInvoicePaymentToDatabaseAsync(InvoicePaymentModel item);
    Task<int> UpdateInvoicePaymentInDatabaseAsync(InvoicePaymentModel item);
    Task<int> DeleteInvoicePaymentFromDatabaseAsync(int ItemId);
    Task<IEnumerable<RecentSellPriceModel>> GetRecentSellPricesAsync(int MaxRecord, int CustomerId, int ProductId);
}