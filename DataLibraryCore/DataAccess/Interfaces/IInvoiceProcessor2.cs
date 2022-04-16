using FluentValidation.Results;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public interface IInvoiceProcessor2<TModel> : IProcessorBase<TModel>
    {
        string GenerateWhereClause(string val, InvoiceLifeStatus? LifeStatus, InvoiceFinancialStatus? FinStatus, SqlSearchMode mode = SqlSearchMode.OR);
        ValidationResult ValidateItem(InvoiceItemModel item);
        ValidationResult ValidateItem(InvoicePaymentModel item);
        Task<int> DeleteInvoiceItemFromDatabaseAsync(int ItemId);
        Task<List<ProductNamesForComboBox>> GetProductItemsAsync(string SearchText);
        Task<double> GetTotalOrRestTotalBalanceOfCustomerAsync(int CustomerId, int InvoiceId = 0);
        Task<InvoiceItemModel> GetInvoiceItemFromDatabaseAsync(int Id);
        Task<int> InsertInvoiceItemToDatabaseAsync(InvoiceItemModel item);
        Task<List<ProductUnitModel>> GetProductUnitsAsync();
        Task<ObservableCollection<InvoiceListModel>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, string OrderBy, OrderType Order);
        Task<int> UpdateInvoiceItemInDatabaseAsync(InvoiceItemModel item);
        Task<InvoicePaymentModel> GetInvoicePaymentFromDatabaseAsync(int Id);
        Task<int> InsertInvoicePaymentToDatabaseAsync(InvoicePaymentModel item);
        Task<int> UpdateInvoicePaymentInDatabaseAsync(InvoicePaymentModel item);
        Task<int> DeleteInvoicePaymentFromDatabaseAsync(int ItemId);
        Task<ObservableCollection<RecentSellPriceModel>> GetRecentSellPricesAsync(int MaxRecord, int CustomerId, int ProductId);
    }
}