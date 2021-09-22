using DataLibraryCore.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public partial interface ICustomerProcessor
    {
        Task<int> CreateItemAsync(CustomerModel customer);
        Task<int> DeleteItemByIdAsync(int Id);
        Task<int> GetTotalQueryCountAsync(string WhereClause);
        Task<ObservableCollection<CustomerModel>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, OrderType Order = OrderType.ASC, string OrderBy = "FirstName");
        Task<CustomerModel> LoadSingleItemAsync(int Id);
        Task<int> UpdateItemAsync(CustomerModel customer);
    }
}