using AvazehWpfApiClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvazehWpfApiClient.DataAccess.Interfaces
{
    public partial interface IChequeProcessor
    {
        Task<int> CreateItemAsync(ChequeModel cheque);
        Task<int> DeleteItemByIdAsync(int Id);
        Task<int> GetTotalQueryCountAsync(string WhereClause);
        Task<ObservableCollection<ChequeModel>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, OrderType Order = OrderType.DESC, string OrderBy = "DueDate");
        Task<ChequeModel> LoadSingleItemAsync(int Id);
        Task<int> UpdateItemAsync(ChequeModel cheque);
    }
}
