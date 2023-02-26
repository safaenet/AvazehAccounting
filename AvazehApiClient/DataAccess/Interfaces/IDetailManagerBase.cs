using FluentValidation.Results;
using System.Threading.Tasks;

namespace AvazehApiClient.DataAccess.Interfaces;

public interface IDetailManagerBase<T>
{
    Task<T> GetItemById(int Id);
    Task<T> CreateItemAsync(T item);
    Task<T> UpdateItemAsync(T item);
    Task<bool> DeleteItemAsync(int Id);
    ValidationResult ValidateItem(T item);
}