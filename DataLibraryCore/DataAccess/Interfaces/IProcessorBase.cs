using FluentValidation.Results;
using SharedLibrary.Enums;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public interface IProcessorBase<T>
    {
        ValidationResult ValidateItem(T model);
        Task<int> CreateItemAsync(T model);
        Task<int> UpdateItemAsync(T model);
        Task<int> DeleteItemByIdAsync(int Id);
        Task<int> GetTotalQueryCountAsync(string WhereClause);
        Task<T> LoadSingleItemAsync(int Id);
    }
}