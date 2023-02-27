using FluentValidation.Results;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces;

public interface ITransactionProcessorBase<TModel> : IProcessorBase<TModel>
{
    string GenerateWhereClause(string val, TransactionFinancialStatus? FinStatus, SqlSearchMode mode = SqlSearchMode.OR);
    string GenerateTransactionItemWhereClause(string val, TransactionFinancialStatus? FinStatus, SqlSearchMode mode);
    Task<List<ItemsForComboBox>> GetProductItemsAsync(string SearchText = null, int TransactionId = 0);
    Task<List<ItemsForComboBox>> GetTransactionNamesAsync(string SearchText = null);
    ValidationResult ValidateItem(TransactionItemModel item);
    Task<int> DeleteTransactionItemFromDatabaseAsync(int ItemId);
    Task<TransactionItemModel> GetTransactionItemFromDatabaseAsync(int Id);
    Task<int> InsertTransactionItemToDatabaseAsync(TransactionItemModel item);
    Task<int> GetTotalTransactionItemQueryCountAsync(string WhereClause, int Id);
    Task<List<TransactionListModel>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, string OrderBy, OrderType Order);
    Task<List<TransactionItemModel>> LoadManyTransactionItemsAsync(int OffSet, int FetcheSize, string WhereClause, int Id, string OrderBy, OrderType Order);
    Task<int> UpdateTransactionItemInDatabaseAsync(TransactionItemModel item);
    Task<double> LoadTotalPositive(int Id);
    Task<double> LoadTotalNegative(int Id);
}