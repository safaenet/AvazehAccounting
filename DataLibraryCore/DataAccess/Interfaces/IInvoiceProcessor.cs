using DataLibraryCore.Models;
using FluentValidation.Results;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public partial interface IInvoiceProcessor
    {
        int CreateItem(InvoiceModel invoice);
        int DeleteItemById(int Id);
        int DeleteSubItemFromDatabase(InvoiceItemModel item);
        Dictionary<int, string> GetProductItems();
        double GetTotalOrRestTotalBalanceOfCustomer(int CustomerID, int InvoiceID = 0);
        int GetTotalQueryCount(string WhereClause);
        int InsertSubItemToDatabase(InvoiceItemModel item);
        ObservableCollection<InvoiceListModel> LoadManyItems(int OffSet, int FetcheSize, string WhereClause, OrderType Order = OrderType.DESC, string OrderBy = "Id");
        InvoiceModel LoadSingleItem(int ID);
        int UpdateItem(InvoiceModel invoice);
        int UpdateSubItemInDatabase(InvoiceItemModel item);
        string GenerateWhereClause(string val, InvoiceLifeStatus? LifeStatus, InvoiceFinancialStatus? FinStatus, SqlSearchMode mode = SqlSearchMode.OR);
        ValidationResult ValidateInvoice(InvoiceModel invoice);
    }
}