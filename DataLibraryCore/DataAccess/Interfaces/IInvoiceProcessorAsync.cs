using DataLibraryCore.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public partial interface IInvoiceProcessor
    {
        Task<int> CreateItemAsync(InvoiceModel invoice);
        Task<int> DeleteItemByIdAsync(int Id);
        Task<int> DeleteSubItemFromDatabaseAsync(InvoiceItemModel item);
        Task<Dictionary<int, string>> GetProductItemsAsync();
        Task<double> GetTotalOrRestTotalBalanceOfCustomerAsync(int CustomerID, int InvoiceID = 0);
        Task<int> GetTotalQueryCountAsync(string WhereClause);
        Task<int> InsertSubItemToDatabaseAsync(InvoiceItemModel item);
        Task<ObservableCollection<InvoiceListModel>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, OrderType Order = OrderType.DESC, string OrderBy = "Id");
        Task<InvoiceModel> LoadSingleItemAsync(int ID);
        Task<int> UpdateItemAsync(InvoiceModel invoice);
        Task<int> UpdateSubItemInDatabaseAsync(InvoiceItemModel item);
    }
}