using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.ObjectModel;
using FluentValidation.Results;
using DataLibraryCore.DataAccess.Interfaces;
using SharedLibrary.DalModels;
using SharedLibrary.Validators;
using SharedLibrary.Enums;
using System.Threading.Tasks;
using Serilog;
using System;
using System.Collections.Generic;
using SharedLibrary.Helpers;

namespace DataLibraryCore.DataAccess.SqlServer;

public class SqlCustomerProcessor<TModel, TSub, TValidator> : IGeneralProcessor<TModel>
    where TModel : CustomerModel where TSub : PhoneNumberModel where TValidator : CustomerValidator, new()
{
    public SqlCustomerProcessor(IDataAccess dataAcess)
    {
        DataAccess = dataAcess;
    }

    private readonly IDataAccess DataAccess;
    private const string QueryOrderBy = "FirstName";
    private const OrderType QueryOrderType = OrderType.ASC;
    private readonly string CreateCustomerQuery = @"DECLARE @newId int; SET @newId = (SELECT ISNULL(MAX([Id]), 0) FROM [Customers]) + 1;
            INSERT INTO Customers ([Id], FirstName, LastName, CompanyName, EmailAddress, PostAddress, DateJoined, Descriptions)
            VALUES (@newId, @firstName, @lastName, @companyName, @emailAddress, @postAddress, @dateJoined, @descriptions);
            SELECT @id = @newId;";
    private readonly string UpdateCustomerQuery = @"UPDATE Customers SET FirstName = @firstName, LastName = @lastName, CompanyName = @companyName,
            EmailAddress = @emailAddress, PostAddress = @postAddress, DateJoined = @dateJoined, Descriptions = @descriptions
            WHERE Id = @id";
    private readonly string InsertPhonesQuery = @"DECLARE @newId int; SET @newId = (SELECT ISNULL(MAX([Id]), 0) FROM [PhoneNumbers]) + 1;
            INSERT INTO PhoneNumbers ([Id], CustomerId, PhoneNumber) VALUES (@newId, @CustomerId, @PhoneNumber)";
    private readonly string SelectCustomersQuery = @"SET NOCOUNT ON
            DECLARE @customers TABLE(
	        [Id] [int],
	        [FirstName] [nvarchar](50),
	        [LastName] [nvarchar](50),
	        [CompanyName] [nvarchar](50),
	        [EmailAddress] [nvarchar](50),
	        [PostAddress] [ntext],
	        [DateJoined] dbo.DateType,
	        [Descriptions] NVARCHAR(MAX))
            {0}
            SELECT * FROM @customers ORDER BY [Id] ASC;
            SELECT * FROM PhoneNumbers WHERE CustomerId IN (SELECT c.Id FROM @customers c);";
    private readonly string DeleteCustomerQuery = @"DELETE FROM Customers WHERE Id = @id";

    public string GenerateWhereClause(string val, SqlSearchMode mode = SqlSearchMode.OR)
    {
        try
        {
            var criteria = string.IsNullOrWhiteSpace(val) ? "'%'" : $"'%{ val }%'";
            return @$"(CAST([Id] AS varchar) LIKE { criteria } 
                             {mode} [FirstName] LIKE N{ criteria } 
                             {mode} [LastName] LIKE N{ criteria } 
                             {mode} [CompanyName] LIKE N{ criteria } 
                             {mode} [EmailAddress] LIKE N{ criteria } 
                             {mode} [PostAddress] LIKE N{ criteria } 
                             {mode} [DateJoined] LIKE { criteria }   
                             {mode} [Descriptions] LIKE N{ criteria } )";
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlCustomerProcessor");
        }
        return null;
    }

    public ValidationResult ValidateItem(TModel customer)
    {
        try
        {
            TValidator validator = new();
            var result = validator.Validate(customer);
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlCustomerProcessor");
        }
        return null;
    }

    public async Task<int> CreateItemAsync(TModel item)
    {
        try
        {
            if (item == null || !ValidateItem(item).IsValid) return 0;
            item.DateJoined = PersianCalendarHelper.GetCurrentPersianDate();
            var dp = new DynamicParameters();
            dp.Add("@id", 0, DbType.Int32, ParameterDirection.Output);
            dp.Add("@firstName", item.FirstName);
            dp.Add("@lastName", item.LastName);
            dp.Add("@companyName", item.CompanyName);
            dp.Add("@emailAddress", item.EmailAddress);
            dp.Add("@postAddress", item.PostAddress);
            dp.Add("@dateJoined", item.DateJoined);
            dp.Add("@descriptions", item.Descriptions);
            var AffectedCount = await DataAccess.SaveDataAsync(CreateCustomerQuery, dp);
            var OutputId = dp.Get<int>("@id");
            if (AffectedCount > 0)
            {
                item.Id = OutputId;
                await InsertPhoneNumbersToDatabaseAsync(item).ConfigureAwait(false);
            }
            return OutputId;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlCustomerProcessor");
        }
        return 0;
    }

    public async Task<int> UpdateItemAsync(TModel item)
    {
        try
        {
            if (item == null || !ValidateItem(item).IsValid) return 0;
            if (item.DateJoined is null) item.DateJoined = PersianCalendarHelper.GetCurrentPersianDate();
            var AffectedCount = await DataAccess.SaveDataAsync(UpdateCustomerQuery, item);
            if (AffectedCount > 0)
            {
                string sqlPhones = $"DELETE FROM PhoneNumbers WHERE CustomerId = { item.Id }";
                await DataAccess.SaveDataAsync<DynamicParameters>(sqlPhones, null).ConfigureAwait(false);
                await InsertPhoneNumbersToDatabaseAsync(item).ConfigureAwait(false);
            }
            return AffectedCount;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlCustomerProcessor");
        }
        return 0;
    }

    private async Task<int> InsertPhoneNumbersToDatabaseAsync(TModel customer)
    {
        try
        {
            if (customer == null || customer.PhoneNumbers == null || customer.PhoneNumbers.Count == 0) return 0;
            ObservableCollection<TSub> phones = new();
            foreach (var phone in customer.PhoneNumbers)
            {
                if (!string.IsNullOrEmpty(phone.PhoneNumber) && !string.IsNullOrWhiteSpace(phone.PhoneNumber))
                {
                    phone.CustomerId = customer.Id;
                    phones.Add(phone as TSub);
                }
            }
            if (phones.Count == 0) return 0;
            return await DataAccess.SaveDataAsync(InsertPhonesQuery, phones).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlCustomerProcessor");
        }
        return 0;
    }

    public async Task<int> DeleteItemByIdAsync(int Id)
    {
        try
        {
            var dp = new DynamicParameters();
            dp.Add("@id", Id);
            return await DataAccess.SaveDataAsync(DeleteCustomerQuery, dp);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlCustomerProcessor");
        }
        return 0;
    }

    public async Task<int> GetTotalQueryCountAsync(string WhereClause)
    {
        try
        {
            var sqlTemp = $@"SELECT COUNT([Id]) FROM Customers
                             { (string.IsNullOrEmpty(WhereClause) ? "" : " WHERE ") } { WhereClause }";
            return await DataAccess.ExecuteScalarAsync<int>(sqlTemp);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlCustomerProcessor");
        }
        return 0;
    }

    public async Task<IEnumerable<TModel>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, string OrderBy = QueryOrderBy, OrderType Order = QueryOrderType)
    {
        try
        {
            var sqlInsert = $@"INSERT @customers SELECT * FROM Customers { (string.IsNullOrEmpty(WhereClause) ? "" : $" WHERE { WhereClause }") }
                               ORDER BY [{OrderBy}] {Order} OFFSET {OffSet} ROWS FETCH NEXT {FetcheSize} ROWS ONLY";
            var query = string.Format(SelectCustomersQuery, sqlInsert);
            using IDbConnection conn = new SqlConnection(DataAccess.GetConnectionString());
            var reader = await conn.QueryMultipleAsync(query, null);
            return await reader.MapObservableCollectionOfCustomersAsync() as List<TModel>;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlCustomerProcessor");
        }
        return null;
    }

    public async Task<TModel> LoadSingleItemAsync(int Id)
    {
        try
        {
            var outPut = await LoadManyItemsAsync(0, 1, $"[Id] = { Id }");
            return outPut.FirstOrDefault();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlCustomerProcessor");
        }
        return null;
    }
}