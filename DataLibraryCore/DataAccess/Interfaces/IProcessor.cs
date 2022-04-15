using FluentValidation.Results;
using SharedLibrary.Enums;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public interface IProcessor<T>
    {
        string GenerateWhereClause(string val, SqlSearchMode mode);
        ValidationResult ValidateItem(T model);

        Task<int> CreateItemAsync(T model);
        Task<int> DeleteItemByIdAsync(int Id);
        Task<int> GetTotalQueryCountAsync(string WhereClause);
        Task<ObservableCollection<T>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, string OrderBy, OrderType Order);
        Task<T> LoadSingleItemAsync(int Id);
        Task<int> UpdateItemAsync(T model);
    }
}