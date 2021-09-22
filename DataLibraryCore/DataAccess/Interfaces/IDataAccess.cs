using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public interface IDataAccess
    {
        string GetConnectionString(string DB = "default");
        ObservableCollection<T> LoadData<T, U>(string sql, U param);
        int SaveData<T>(string sql, T data);
        T ExecuteScalar<T, U>(string sql, U param);
        Task<ObservableCollection<T>> LoadDataAsync<T, U>(string sql, U param);
        Task<int> SaveDataAsync<T>(string sql, T data);
        Task<T> ExecuteScalarAsync<T, U>(string sql, U param);
    }
}