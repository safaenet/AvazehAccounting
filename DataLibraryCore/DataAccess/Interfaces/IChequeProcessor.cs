using DataLibraryCore.Models;
using FluentValidation.Results;
using System.Collections.ObjectModel;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public interface IChequeProcessor
    {
        int CreateItem(ChequeModel cheque);
        int DeleteItemByID(int ID);
        int GetTotalQueryCount(string WhereClause);
        ObservableCollection<ChequeModel> LoadManyItems(int OffSet, int FetcheSize, string WhereClause, OrderType Order = OrderType.DESC, string OrderBy = "DueDate");
        ChequeModel LoadSingleItem(int ID);
        int UpdateItem(ChequeModel cheque);
        string GenerateWhereClause(string val, SqlSearchMode mode = SqlSearchMode.OR);
        ValidationResult ValidateItem(ChequeModel cheque);
    }
}