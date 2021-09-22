using Dapper;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using DataLibraryCore.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.SqlServer
{
    public class SqlDataAccess : Interfaces.IDataAccess
    {
        public string GetConnectionString(string DB = "secondary") => SettingsDataAccess.AppConfiguration().GetConnectionString(DB);

        public ObservableCollection<T> LoadData<T, U>(string sql, U param)
        {
            using IDbConnection conn = new SqlConnection(GetConnectionString());
            return conn.Query<T>(sql, param).AsObservable();
        }

        public async Task<ObservableCollection<T>> LoadDataAsync<T, U>(string sql, U param)
        {
            using IDbConnection conn = new SqlConnection(GetConnectionString());
            return await conn.QueryAsync<T>(sql, param).AsObservableAsync();
        }

        public int SaveData<T>(string sql, T data)
        {
            using IDbConnection conn = new SqlConnection(GetConnectionString());
            return conn.Execute(sql, data);
        }

        public async Task<int> SaveDataAsync<T>(string sql, T data)
        {
            using IDbConnection conn = new SqlConnection(GetConnectionString());
            return await conn.ExecuteAsync(sql, data);
        }

        public T ExecuteScalar<T, U>(string sql, U param)
        {
            using IDbConnection conn = new SqlConnection(GetConnectionString());
            return conn.ExecuteScalar<T>(sql, param);
        }

        public async Task<T> ExecuteScalarAsync<T, U>(string sql, U param)
        {
            using IDbConnection conn = new SqlConnection(GetConnectionString());
            return await conn.ExecuteScalarAsync<T>(sql, param);
        }
    }
}