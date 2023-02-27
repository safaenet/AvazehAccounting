using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces;

public interface IDataAccess
{
    Task<bool> TestConnectionAsync();
    string GetConnectionString(string DB = "default");
    Task<List<T>> LoadDataAsync<T, U>(string sql, U param);
    Task<List<T>> LoadDataAsync<T>(string sql);
    Task<int> SaveDataAsync<T>(string sql, T data);
    Task<int> SaveDataAsync(string sql);
    Task<T> ExecuteScalarAsync<T, U>(string sql, U param);
    Task<T> ExecuteScalarAsync<T>(string sql);
    Task<T> QuerySingleOrDefaultAsync<T, U>(string sql, U param);
}