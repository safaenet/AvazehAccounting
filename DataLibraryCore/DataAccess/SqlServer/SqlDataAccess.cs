using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;
using Serilog;

namespace DataLibraryCore.DataAccess.SqlServer;

public class SqlDataAccess : Interfaces.IDataAccess
{
    public string GetConnectionString(string DB = "default") => SettingsDataAccess.AppConfiguration().GetConnectionString(DB);

    private bool TestConnection()
    {
        using IDbConnection conn = new SqlConnection(GetConnectionString());
        System.Data.Common.DbConnectionStringBuilder csb = new();
        try
        {
            csb.ConnectionString = GetConnectionString();
            conn.Open();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlDataAccess");
            return false;
        }
        return true;
    }

    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            var task = Task.Run(() => TestConnection());
            return await task;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlDataAccess");
        }
        return false;
    }

    public async Task<ObservableCollection<T>> LoadDataAsync<T, U>(string sql, U param)
    {
        try
        {
            using IDbConnection conn = new SqlConnection(GetConnectionString());
            return await conn.QueryAsync<T>(sql, param).AsObservableAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlDataAccess");
        }
        return null;
    }

    public async Task<int> SaveDataAsync<T>(string sql, T data)
    {
        try
        {
            using IDbConnection conn = new SqlConnection(GetConnectionString());
            var result = await conn.ExecuteAsync(sql, data);
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlDataAccess");
        }
        return 0;
    }

    public async Task<T> ExecuteScalarAsync<T, U>(string sql, U param)
    {
        try
        {
            using IDbConnection conn = new SqlConnection(GetConnectionString());
            return await conn.ExecuteScalarAsync<T>(sql, param);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlDataAccess");
        }
        return default;
    }

    public async Task<T> QuerySingleOrDefaultAsync<T, U>(string sql, U param)
    {
        try
        {
            using IDbConnection conn = new SqlConnection(GetConnectionString());
            return await conn.QuerySingleOrDefaultAsync<T>(sql, param);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlDataAccess");
        }
        return default;
    }
}