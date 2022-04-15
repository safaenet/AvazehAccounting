﻿using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.ObjectModel;
using FluentValidation.Results;
using DataLibraryCore.DataAccess.Interfaces;
using SharedLibrary.DalModels;
using SharedLibrary.Validators;
using SharedLibrary.Enums;
using DataLibraryCore.Models;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.SqlServer
{
    public class SqlCustomerProcessor<TModel, TSub, TValidator> : IProcessor<TModel>
        where TModel : CustomerModel where TSub : PhoneNumberModel where TValidator : CustomerValidator, new()
    {
        public SqlCustomerProcessor(IDataAccess dataAcess)
        {
            DataAccess = dataAcess;
        }

        private readonly IDataAccess DataAccess;
        private const string QueryOrderBy = "FirstName";
        private const OrderType QueryOrderType = OrderType.ASC;
        private readonly string CreateCustomerQuery = @"INSERT INTO Customers (FirstName, LastName, CompanyName, EmailAddress, PostAddress, DateJoined, Descriptions)
            VALUES (@firstName, @lastName, @companyName, @emailAddress, @postAddress, @dateJoined, @descriptions);
            SELECT @id = @@IDENTITY;";
        private readonly string UpdateCustomerQuery = @"UPDATE Customers SET FirstName = @firstName, LastName = @lastName, CompanyName = @companyName,
            EmailAddress = @emailAddress, PostAddress = @postAddress, DateJoined = @dateJoined, Descriptions = @descriptions
            WHERE Id = @id";
        private readonly string InsertPhonesQuery = @"INSERT INTO PhoneNumbers (CustomerId, PhoneNumber) VALUES (@CustomerId, @PhoneNumber)";
        private readonly string SelectCustomersQuery = @"SET NOCOUNT ON
            DECLARE @customers TABLE(
	        [Id] [int],
	        [FirstName] [nvarchar](50),
	        [LastName] [nvarchar](50),
	        [CompanyName] [nvarchar](50),
	        [EmailAddress] [nvarchar](50),
	        [PostAddress] [ntext],
	        [DateJoined] [char](10),
	        [Descriptions] [ntext])
            {0}
            SELECT * FROM @customers ORDER BY [Id] ASC;
            SELECT * FROM PhoneNumbers WHERE CustomerId IN (SELECT c.Id FROM @customers c);";
        private readonly string DeleteCustomerQuery = @"DELETE FROM Customers WHERE Id = @id";

        public string GenerateWhereClause(string val, SqlSearchMode mode = SqlSearchMode.OR)
        {
            var criteria = string.IsNullOrWhiteSpace(val) ? "'%'" : $"'%{ val }%'";
            return @$"(CAST([Id] AS varchar) LIKE { criteria } 
                             {mode} [FirstName] LIKE { criteria } 
                             {mode} [LastName] LIKE { criteria } 
                             {mode} [CompanyName] LIKE { criteria } 
                             {mode} [EmailAddress] LIKE { criteria } 
                             {mode} [PostAddress] LIKE { criteria } 
                             {mode} [DateJoined] LIKE { criteria }  
                             {mode} [Descriptions] LIKE { criteria } )";
        }

        public ValidationResult ValidateItem(TModel customer)
        {
            TValidator validator = new();
            var result = validator.Validate(customer);
            return result;
        }

        public async Task<int> CreateItemAsync(TModel item)
        {
            if (item == null || !ValidateItem(item).IsValid) return 0;
            item.DateJoined = PersianCalendarModel.GetCurrentPersianDate();
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

        public async Task<int> UpdateItemAsync(TModel item)
        {
            if (item == null || !ValidateItem(item).IsValid) return 0;
            if (item.DateJoined is null) item.DateJoined = PersianCalendarModel.GetCurrentPersianDate();
            var AffectedCount = await DataAccess.SaveDataAsync(UpdateCustomerQuery, item);
            if (AffectedCount > 0)
            {
                string sqlPhones = $"DELETE FROM PhoneNumbers WHERE CustomerId = { item.Id }";
                await DataAccess.SaveDataAsync<DynamicParameters>(sqlPhones, null).ConfigureAwait(false);
                await InsertPhoneNumbersToDatabaseAsync(item).ConfigureAwait(false);
            }
            return AffectedCount;
        }

        private async Task<int> InsertPhoneNumbersToDatabaseAsync(TModel customer)
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

        public async Task<int> DeleteItemByIdAsync(int Id)
        {
            var dp = new DynamicParameters();
            dp.Add("@id", Id);
            return await DataAccess.SaveDataAsync(DeleteCustomerQuery, dp);
        }

        public async Task<int> GetTotalQueryCountAsync(string WhereClause)
        {
            var sqlTemp = $@"SELECT COUNT([Id]) FROM Customers
                             { (string.IsNullOrEmpty(WhereClause) ? "" : " WHERE ") } { WhereClause }";
            return await DataAccess.ExecuteScalarAsync<int, DynamicParameters>(sqlTemp, null);
        }

        public async Task<ObservableCollection<TModel>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, string OrderBy = QueryOrderBy, OrderType Order = QueryOrderType)
        {
            var sqlInsert = $@"INSERT @customers SELECT * FROM Customers
                               { (string.IsNullOrEmpty(WhereClause) ? "" : $" WHERE { WhereClause }") }
                               ORDER BY [{OrderBy}] {Order} OFFSET {OffSet} ROWS FETCH NEXT {FetcheSize} ROWS ONLY";
            var query = string.Format(SelectCustomersQuery, sqlInsert);
            using IDbConnection conn = new SqlConnection(DataAccess.GetConnectionString());
            var reader = await conn.QueryMultipleAsync(query, null);
            return await reader.MapObservableCollectionOfCustomersAsync() as ObservableCollection<TModel>;
        }

        public async Task<TModel> LoadSingleItemAsync(int Id)
        {
            var outPut = await LoadManyItemsAsync(0, 1, $"[Id] = { Id }");
            return outPut.FirstOrDefault();
        }
    }
}