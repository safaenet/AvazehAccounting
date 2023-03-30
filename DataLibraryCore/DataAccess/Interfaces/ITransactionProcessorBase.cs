using FluentValidation.Results;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces;

public interface ITransactionProcessorBase<TModel> : IProcessorBase<TModel>
{
    string GenerateWhereClause(string val, TransactionFinancialStatus? FinStatus, int Id, string Date, SqlSearchMode mode);
    string GenerateTransactionItemWhereClause(string val, TransactionFinancialStatus? FinStatus, string Date, SqlSearchMode mode);
    Task<IEnumerable<ItemsForComboBox>> GetProductItemsAsync(string SearchText = null, int TransactionId = 0);
    Task<IEnumerable<ItemsForComboBox>> GetTransactionNamesAsync(string SearchText = null);
    ValidationResult ValidateItem(TransactionItemModel item);
    Task<int> DeleteTransactionItemFromDatabaseAsync(int ItemId);
    Task<TransactionItemModel> GetTransactionItemFromDatabaseAsync(int Id);
    Task<int> InsertTransactionItemToDatabaseAsync(TransactionItemModel item);
    Task<int> GetTotalTransactionItemQueryCountAsync(string WhereClause, int Id);
    Task<IEnumerable<TransactionListModel>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, string OrderBy, OrderType Order);
    Task<IEnumerable<TransactionItemModel>> LoadManyTransactionItemsAsync(int OffSet, int FetcheSize, string WhereClause, int Id, string OrderBy, OrderType Order);
    Task<int> UpdateTransactionItemInDatabaseAsync(TransactionItemModel item);
    Task<decimal> LoadTotalPositive(int Id);
    Task<decimal> LoadTotalNegative(int Id);
}