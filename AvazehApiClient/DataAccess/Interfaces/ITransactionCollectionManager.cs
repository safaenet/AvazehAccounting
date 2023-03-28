using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AvazehApiClient.DataAccess.Interfaces;

public interface ITransactionCollectionManager : ICollectionManagerBase<TransactionModel>
{
    ObservableCollection<TransactionListModel> Items { get; set; }
    TransactionFinancialStatus? FinStatus { get; set; }
    TransactionListModel GetItemFromCollectionById(int Id);
    Task<List<ItemsForComboBox>> LoadProductItems(string SearchText = null);
    public int TransactionIdToSearch { get; set; }
    public string TransactionDateToSearch { get; set; }
}