using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AvazehApiClient.DataAccess.Interfaces;

public interface IInvoiceCollectionManager : ICollectionManagerBase<InvoiceModel>
{
    ObservableCollection<InvoiceListModel> Items { get; set; }
    InvoiceLifeStatus? LifeStatus { get; set; }
    InvoiceFinancialStatus? FinStatus { get; set; }
    InvoiceListModel GetItemFromCollectionById(int Id);
    Task<List<ItemsForComboBox>> LoadProductItems(string SearchText = null);
    Task<double> GetCustomerTotalBalanceById(int CustomerId, int InvoiceId = 0);
    Task<List<UserDescriptionModel>> GetUserDescriptions();
}