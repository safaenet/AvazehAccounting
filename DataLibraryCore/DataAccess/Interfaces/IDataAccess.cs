using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces;

public interface IDataAccess
{
    Task<bool> TestConnectionAsync();
    string GetConnectionString(string DB = "default");
    Task<IEnumerable<T>> LoadDataAsync<T, U>(string sql, U param, CommandType type = CommandType.Text);
    Task<IEnumerable<T>> LoadDataAsync<T>(string sql, CommandType CType = CommandType.Text);
    Task<int> SaveDataAsync<T>(string sql, T data);
    Task<int> SaveDataAsync(string sql);
    Task<T> ExecuteScalarAsync<T, U>(string sql, U param);
    Task<T> ExecuteScalarAsync<T>(string sql);
    Task<T> QuerySingleOrDefaultAsync<T, U>(string sql, U param);
}