using DataLibraryCore.Models;
using FluentValidation.Results;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public interface IProcessor<T>
    {
        int CreateItem(T cheque);
        int DeleteItemById(int Id);
        int GetTotalQueryCount(string WhereClause);
        ObservableCollection<T> LoadManyItems(int OffSet, int FetcheSize, string WhereClause, OrderType Order = OrderType.DESC, string OrderBy = "DueDate");
        T LoadSingleItem(int Id);
        int UpdateItem(T cheque);
        string GenerateWhereClause(string val, SqlSearchMode mode = SqlSearchMode.OR);
        ValidationResult ValidateItem(T cheque);

        Task<int> CreateItemAsync(T cheque);
        Task<int> DeleteItemByIdAsync(int Id);
        Task<int> GetTotalQueryCountAsync(string WhereClause);
        Task<ObservableCollection<T>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, OrderType Order = OrderType.DESC, string OrderBy = "DueDate");
        Task<T> LoadSingleItemAsync(int Id);
        Task<int> UpdateItemAsync(T cheque);
    }
}