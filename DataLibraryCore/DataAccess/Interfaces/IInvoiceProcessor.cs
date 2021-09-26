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
        int DeleteSubItemFromDatabase(InvoiceItemModel item);
        Dictionary<int, string> GetProductItems();
        double GetTotalOrRestTotalBalanceOfCustomer(int CustomerId, int InvoiceId = 0);
        int GetTotalQueryCount(string WhereClause);
        int InsertSubItemToDatabase(InvoiceItemModel item);
        ObservableCollection<InvoiceListModel> LoadManyItems(int OffSet, int FetcheSize, string WhereClause, string OrderBy, OrderType Order);
        InvoiceModel LoadSingleItem(int Id);
        int UpdateItem(InvoiceModel invoice);
        int UpdateSubItemInDatabase(InvoiceItemModel item);
        string GenerateWhereClause(string val, InvoiceLifeStatus? LifeStatus, InvoiceFinancialStatus? FinStatus, SqlSearchMode mode = SqlSearchMode.OR);
        ValidationResult ValidateItem(InvoiceModel invoice);

        Task<int> CreateItemAsync(InvoiceModel invoice);
        Task<int> DeleteItemByIdAsync(int Id);
        Task<int> DeleteSubItemFromDatabaseAsync(InvoiceItemModel item);
        Task<Dictionary<int, string>> GetProductItemsAsync();
        Task<double> GetTotalOrRestTotalBalanceOfCustomerAsync(int CustomerId, int InvoiceId = 0);
        Task<int> GetTotalQueryCountAsync(string WhereClause);
        Task<int> InsertSubItemToDatabaseAsync(InvoiceItemModel item);
        Task<ObservableCollection<InvoiceListModel>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, string OrderBy, OrderType Order);
        Task<InvoiceModel> LoadSingleItemAsync(int Id);
        Task<int> UpdateItemAsync(InvoiceModel invoice);
        Task<int> UpdateSubItemInDatabaseAsync(InvoiceItemModel item);
    }
}