using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public interface IDataAccess
    {
        string GetConnectionString(string DB = "default");
        Task<ObservableCollection<T>> LoadDataAsync<T, U>(string sql, U param);
        Task<int> SaveDataAsync<T>(string sql, T data);
        Task<T> ExecuteScalarAsync<T, U>(string sql, U param);
        Task<T> QuerySingleOrDefaultAsync<T, U>(string sql, U param);
    }
}