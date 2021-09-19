using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public interface IDataAccess
    {
        T ExecuteScalar<T, U>(string sql, U param);
        Task<T> ExecuteScalarAsync<T, U>(string sql, U param);
        string GetConnectionString(string DB = "default");
        ObservableCollection<T> LoadData<T, U>(string sql, U param);
        Task<ObservableCollection<T>> LoadDataAsync<T, U>(string sql, U param);
        int SaveData<T>(string sql, T data);
        Task<int> SaveDataAsync<T>(string sql, T data);
    }
}