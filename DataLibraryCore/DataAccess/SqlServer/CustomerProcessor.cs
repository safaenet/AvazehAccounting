﻿using Dapper;
using DataLibraryCore.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using DataLibraryCore.DataAccess.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.ObjectModel;
using DataLibraryCore.Models.Validators;
using FluentValidation.Results;
using DataLibraryCore.DataAccess.Interfaces;

namespace DataLibraryCore.DataAccess.SqlServer
{
    public class SqlCustomerProcessor : ICustomerProcessor
    {
        public SqlCustomerProcessor(IDataAccess dataAcess)
        {
            DataAccess = dataAcess;
        }

        private readonly IDataAccess DataAccess;
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

        public int CreateItem(CustomerModel customer)
        {
            if (customer == null) return 0;
            var dp = new DynamicParameters();
            dp.Add("@id", 0, DbType.Int32, ParameterDirection.Output);
            dp.Add("@firstName", customer.FirstName);
            dp.Add("@lastName", customer.LastName);
            dp.Add("@companyName", customer.CompanyName);
            dp.Add("@emailAddress", customer.EmailAddress);
            dp.Add("@postAddress", customer.PostAddress);
            dp.Add("@dateJoined", customer.DateJoined);
            dp.Add("@descriptions", customer.Descriptions);
            var AffectedCount = DataAccess.SaveDataAsync(CreateCustomerQuery, dp);
            var OutputId = dp.Get<int>("@id");
            if (AffectedCount.Result > 0)
            {
                customer.Id = OutputId;
                InsertPhoneNumbersToDatabase(customer);
            }
            return OutputId;
        }

        public int UpdateItem(CustomerModel customer)
        {
            if (customer == null) return 0;
            var AffectedCount = DataAccess.SaveData(UpdateCustomerQuery, customer);
            if (AffectedCount > 0)
            {
                string sqlPhones = $"DELETE FROM PhoneNumbers WHERE CustomerId = { customer.Id }";
                DataAccess.SaveData<DynamicParameters>(sqlPhones, null);
                InsertPhoneNumbersToDatabase(customer);
            }
            return AffectedCount;
        }

        private int InsertPhoneNumbersToDatabase(CustomerModel customer)
        {
            if (customer == null || customer.PhoneNumbers == null || customer.PhoneNumbers.Count == 0) return 0;
            ObservableCollection<PhoneNumberModel> phones = new();
            foreach (var phone in customer.PhoneNumbers)
            {
                if (!string.IsNullOrEmpty(phone.PhoneNumber) && !string.IsNullOrWhiteSpace(phone.PhoneNumber))
                {
                    phone.CustomerId = customer.Id;
                    phones.Add(phone);
                }
            }
            if (phones.Count == 0) return 0;
            return DataAccess.SaveData(InsertPhonesQuery, phones);
        }

        public int DeleteItemByID(int ID)
        {
            var dp = new DynamicParameters();
            dp.Add("@id", ID);
            return DataAccess.SaveData(DeleteCustomerQuery, dp);
        }

        public int GetTotalQueryCount(string WhereClause)
        {
            var sqlTemp = $@"SELECT COUNT([Id]) FROM Customers
                             { (string.IsNullOrEmpty(WhereClause) ? "" : " WHERE ") } { WhereClause }";
            return DataAccess.ExecuteScalar<int, DynamicParameters>(sqlTemp, null);
        }

        public ObservableCollection<CustomerModel> LoadManyItems(int OffSet, int FetcheSize, string WhereClause, OrderType Order = OrderType.ASC, string OrderBy = "FirstName")
        {
            var sqlInsert = $@"INSERT @customers SELECT * FROM Customers
                               { (string.IsNullOrEmpty(WhereClause) ? "" : $" WHERE { WhereClause }") }
                               ORDER BY [{OrderBy}] {Order} OFFSET {OffSet} ROWS FETCH NEXT {FetcheSize} ROWS ONLY";
            var query = string.Format(SelectCustomersQuery, sqlInsert);
            using IDbConnection conn = new SqlConnection(DataAccess.GetConnectionString());
            var reader = conn.QueryMultipleAsync(query, null).Result;
            return reader.MapObservableCollectionOfCustomers();
        }

        public CustomerModel LoadSingleItem(int ID)
        {
            return LoadManyItems(0, 1, $"[Id] = { ID }").FirstOrDefault();
        }

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

        public ValidationResult ValidateItem(CustomerModel customer)
        {
            CustomerValidator validator = new();
            var result = validator.Validate(customer);
            return result;
        }
    }
}