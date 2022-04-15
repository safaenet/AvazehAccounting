using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.SqlServer
{
    public class SqlDataAccess : Interfaces.IDataAccess
    {
        public string GetConnectionString(string DB = "default") => SettingsDataAccess.AppConfiguration().GetConnectionString(DB);

        public async Task<ObservableCollection<T>> LoadDataAsync<T, U>(string sql, U param)
        {
            using IDbConnection conn = new SqlConnection(GetConnectionString());
            return await conn.QueryAsync<T>(sql, param).AsObservableAsync();
        }

        public async Task<int> SaveDataAsync<T>(string sql, T data)
        {
            using IDbConnection conn = new SqlConnection(GetConnectionString());
            var result = await conn.ExecuteAsync(sql, data);
            return result;
        }

        public async Task<T> ExecuteScalarAsync<T, U>(string sql, U param)
        {
            using IDbConnection conn = new SqlConnection(GetConnectionString());
            return await conn.ExecuteScalarAsync<T>(sql, param);
        }
    }
}