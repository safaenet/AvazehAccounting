using FluentValidation.Results;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public interface ITransactionProcessorBase<TModel> : IProcessorBase<TModel>
    {
        string GenerateWhereClause(string val, TransactionFinancialStatus? FinStatus, SqlSearchMode mode = SqlSearchMode.OR);
        Task<List<ProductNamesForComboBox>> GetProductItemsAsync(string SearchText = null);
        ValidationResult ValidateItem(TransactionItemModel item);
        Task<int> DeleteTransactionItemFromDatabaseAsync(int ItemId);
        Task<TransactionItemModel> GetTransactionItemFromDatabaseAsync(int Id);
        Task<int> InsertTransactionItemToDatabaseAsync(TransactionItemModel item);
        Task<ObservableCollection<TransactionListModel>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, string OrderBy, OrderType Order);
        Task<int> UpdateTransactionItemInDatabaseAsync(TransactionItemModel item);
    }
}