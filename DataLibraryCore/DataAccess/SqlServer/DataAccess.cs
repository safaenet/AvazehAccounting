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
    public static class DataAccess
    {
        public static string GetConnectionString(string DB = "default") => SettingsDataAccess.AppConfiguration().GetConnectionString(DB);

        public static ObservableCollection<T> LoadData<T, U>(string sql, U param)
        {
            using IDbConnection conn = new SqlConnection(GetConnectionString());
            return new ObservableCollection<T>(conn.Query<T>(sql, param));
        }

        public static async Task<ObservableCollection<T>> LoadDataAsync<T, U>(string sql, U param)
        {
            using IDbConnection conn = new SqlConnection(GetConnectionString());
            return new ObservableCollection<T>(await conn.QueryAsync<T>(sql, param));
        }

        public static int SaveData<T>(string sql, T data)
        {
            using IDbConnection conn = new SqlConnection(GetConnectionString());
            return conn.Execute(sql, data);
        }

        public static async Task<int> SaveDataAsync<T>(string sql, T data)
        {
            using IDbConnection conn = new SqlConnection(GetConnectionString());
            return await conn.ExecuteAsync(sql, data);
        }

        public static T ExecuteScalar<T, U>(string sql, U param)
        {
            using IDbConnection conn = new SqlConnection(GetConnectionString());
            return conn.ExecuteScalar<T>(sql, param);
        }

        public static async Task<T> ExecuteScalarAsync<T, U>(string sql, U param)
        {
            using IDbConnection conn = new SqlConnection(GetConnectionString());
            return await conn.ExecuteScalarAsync<T>(sql, param);
        }
    }
}