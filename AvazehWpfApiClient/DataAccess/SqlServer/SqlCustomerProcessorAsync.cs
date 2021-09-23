using Dapper;
using AvazehWpfApiClient.DataAccess.Interfaces;
using AvazehWpfApiClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvazehWpfApiClient.DataAccess.SqlServer
{
    public partial class SqlCustomerProcessor : ICustomerProcessor
    {
        public async Task<int> CreateItemAsync(CustomerModel item)
        {
            if (item == null || !ValidateItem(item).IsValid) return 0;
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

        public async Task<int> UpdateItemAsync(CustomerModel item)
        {
            if (item == null || !ValidateItem(item).IsValid) return 0;
            var AffectedCount = await DataAccess.SaveDataAsync(UpdateCustomerQuery, item);
            if (AffectedCount > 0)
            {
                string sqlPhones = $"DELETE FROM PhoneNumbers WHERE CustomerId = { item.Id }";
                await DataAccess.SaveDataAsync<DynamicParameters>(sqlPhones, null).ConfigureAwait(false);
                await InsertPhoneNumbersToDatabaseAsync(item).ConfigureAwait(false);
            }
            return AffectedCount;
        }

        private async Task<int> InsertPhoneNumbersToDatabaseAsync(CustomerModel customer)
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

        public async Task<ObservableCollection<CustomerModel>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, OrderType Order = OrderType.ASC, string OrderBy = "FirstName")
        {
            var sqlInsert = $@"INSERT @customers SELECT * FROM Customers
                               { (string.IsNullOrEmpty(WhereClause) ? "" : $" WHERE { WhereClause }") }
                               ORDER BY [{OrderBy}] {Order} OFFSET {OffSet} ROWS FETCH NEXT {FetcheSize} ROWS ONLY";
            var query = string.Format(SelectCustomersQuery, sqlInsert);
            using IDbConnection conn = new SqlConnection(DataAccess.GetConnectionString());
            var reader = await conn.QueryMultipleAsync(query, null);
            return await reader.MapObservableCollectionOfCustomersAsync();
        }

        public async Task<CustomerModel> LoadSingleItemAsync(int Id)
        {
            var outPut = await LoadManyItemsAsync(0, 1, $"[Id] = { Id }");
            return outPut.FirstOrDefault();
        }
    }
}