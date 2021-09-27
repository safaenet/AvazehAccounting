using DataLibraryCore.Models;
using FluentValidation.Results;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public interface IInvoiceProcessor
    {
        int CreateItem(InvoiceModel invoice);
        int DeleteItemById(int Id);
        int DeleteInvoiceItemFromDatabase(int ItemId);
        Dictionary<int, string> GetProductItems();
        double GetTotalOrRestTotalBalanceOfCustomer(int CustomerId, int InvoiceId = 0);
        int GetTotalQueryCount(string WhereClause);
        InvoiceItemModel GetInvoiceItemFromDatabase(int Id);
        int InsertInvoiceItemToDatabase(InvoiceItemModel item);
        ObservableCollection<InvoiceListModel> LoadManyItems(int OffSet, int FetcheSize, string WhereClause, string OrderBy, OrderType Order);
        InvoiceModel LoadSingleItem(int Id);
        int UpdateItem(InvoiceModel invoice);
        int UpdateInvoiceItemInDatabase(InvoiceItemModel item);
        InvoicePaymentModel GetInvoicePaymentFromDatabase(int Id);
        int InsertInvoicePaymentToDatabase(InvoicePaymentModel item);
        int UpdateInvoicePaymentInDatabase(InvoicePaymentModel item);
        int DeleteInvoicePaymentFromDatabase(int PaymentId);
        string GenerateWhereClause(string val, InvoiceLifeStatus? LifeStatus, InvoiceFinancialStatus? FinStatus, SqlSearchMode mode = SqlSearchMode.OR);
        ValidationResult ValidateItem(InvoiceModel item);
        ValidationResult ValidateItem(InvoiceItemModel item);
        ValidationResult ValidateItem(InvoicePaymentModel item);

        Task<int> CreateItemAsync(InvoiceModel invoice);
        Task<int> DeleteItemByIdAsync(int Id);
        Task<int> DeleteInvoiceItemFromDatabaseAsync(int ItemId);
        Task<Dictionary<int, string>> GetProductItemsAsync();
        Task<double> GetTotalOrRestTotalBalanceOfCustomerAsync(int CustomerId, int InvoiceId = 0);
        Task<int> GetTotalQueryCountAsync(string WhereClause);
        Task<InvoiceItemModel> GetInvoiceItemFromDatabaseAsync(int Id);
        Task<int> InsertInvoiceItemToDatabaseAsync(InvoiceItemModel item);
        Task<ObservableCollection<InvoiceListModel>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, string OrderBy, OrderType Order);
        Task<InvoiceModel> LoadSingleItemAsync(int Id);
        Task<int> UpdateItemAsync(InvoiceModel invoice);
        Task<int> UpdateInvoiceItemInDatabaseAsync(InvoiceItemModel item);
        Task<InvoicePaymentModel> GetInvoicePaymentFromDatabaseAsync(int Id);
        Task<int> InsertInvoicePaymentToDatabaseAsync(InvoicePaymentModel item);
        Task<int> UpdateInvoicePaymentInDatabaseAsync(InvoicePaymentModel item);
        Task<int> DeleteInvoicePaymentFromDatabaseAsync(int ItemId);
    }
}