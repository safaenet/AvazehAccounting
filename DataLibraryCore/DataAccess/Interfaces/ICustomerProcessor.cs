using DataLibraryCore.Models;
using FluentValidation.Results;
using System.Collections.ObjectModel;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public interface ICustomerProcessor
    {
        int CreateItem(CustomerModel customer);
        int DeleteItemByID(int Id);
        int GetTotalQueryCount(string WhereClause);
        ObservableCollection<CustomerModel> LoadManyItems(int OffSet, int FetcheSize, string WhereClause, OrderType Order = OrderType.ASC, string OrderBy = "FirstName");
        CustomerModel LoadSingleItem(int ID);
        int UpdateItem(CustomerModel customer);
        string GenerateWhereClause(string val, SqlSearchMode mode = SqlSearchMode.OR);
        ValidationResult ValidateItem(CustomerModel customer);
    }
}