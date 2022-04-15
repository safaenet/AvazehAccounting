using FluentValidation.Results;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public interface ITransactionProcessor
    {
        string GenerateWhereClause(string val, SqlSearchMode mode = SqlSearchMode.OR);
        ValidationResult ValidateItem(TransactionModel item);
        ValidationResult ValidateItem(TransactionItemModel item);
        Task<int> CreateItemAsync(TransactionModel invoice);
        Task<int> DeleteItemByIdAsync(int Id);
        Task<int> DeleteTransactionItemFromDatabaseAsync(int ItemId);
        Task<int> GetTotalQueryCountAsync(string WhereClause);
        Task<TransactionItemModel> GetTransactionItemFromDatabaseAsync(int Id);
        Task<int> InsertTransactionItemToDatabaseAsync(TransactionItemModel item);
        Task<ObservableCollection<TransactionModel>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, string OrderBy, OrderType Order);
        Task<TransactionModel> LoadSingleItemAsync(int Id);
        Task<int> UpdateItemAsync(TransactionModel invoice);
        Task<int> UpdateTransactionItemInDatabaseAsync(TransactionItemModel item);
    }
}