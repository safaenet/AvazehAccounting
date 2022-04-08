using SharedLibrary.Enums;
using System.Threading.Tasks;

namespace AvazehApiClient.DataAccess.Interfaces
{
    public interface IApiProcessor
    {
        Task<T> GetCollectionAsync<T>(string Key, string OrderBy, OrderType orderType, int Page = 1, string SearchText = "", int PageSize = 50, bool ForceLoad = false) where T : class;
        Task<T> GetCollectionAsync<T>(string Key, string SearchText) where T : class;
        Task<T> GetCollectionAsync<T>(string Key, int Id1, int Id2, int Id3) where T : class;
        Task<T> GetInvoiceCollectionAsync<T>(string Key, string OrderBy, OrderType orderType, int Page = 1, string SearchText = "", InvoiceLifeStatus? LifeStatus = null, InvoiceFinancialStatus? FinStatus = null, int PageSize = 50, bool ForceLoad = false) where T : class;
        Task<T> GetItemAsync<T>(string Key, string Id) where T : class;
        Task<U> CreateItemAsync<T, U>(string Key, T model) where U : class;
        Task<U> UpdateItemAsync<T, U>(string Key, int Id, T model) where U : class;
        Task<bool> DeleteItemAsync(string Key, int Id);
        Task<T?> GetValueOrNullAsync<T>(string Key, int Id1, int Id2) where T : struct;
        Task<double> GetValueOrZeroAsync<T>(string Key, int Id1, int Id2);
    }
}