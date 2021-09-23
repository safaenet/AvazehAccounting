using AvazehWpfApiClient.Models;
using FluentValidation.Results;
using System.Collections.ObjectModel;

namespace AvazehWpfApiClient.DataAccess.Interfaces
{
    public partial interface IChequeProcessor
    {
        int CreateItem(ChequeModel cheque);
        int DeleteItemById(int Id);
        int GetTotalQueryCount(string WhereClause);
        ObservableCollection<ChequeModel> LoadManyItems(int OffSet, int FetcheSize, string WhereClause, OrderType Order = OrderType.DESC, string OrderBy = "DueDate");
        ChequeModel LoadSingleItem(int Id);
        int UpdateItem(ChequeModel cheque);
        string GenerateWhereClause(string val, SqlSearchMode mode = SqlSearchMode.OR);
        ValidationResult ValidateItem(ChequeModel cheque);
    }
}