using AvazehApiClient.DataAccess.Interfaces;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvazehApiClient.DataAccess
{
    public class SingletonClass
    {
        public SingletonClass(IApiProcessor processor)
        {
            Processor = processor;
        }

        private readonly IApiProcessor Processor;

        public async Task<ObservableCollection<ItemsForComboBox>> ReloadProductNames() => await Processor.GetCollectionAsync<ObservableCollection<ItemsForComboBox>>("Invoices/ProductItems", null);

        public async Task<ObservableCollection<ItemsForComboBox>> ReloadProductNamesAndTransactionItems(int TransactionId = 0) => await Processor.GetCollectionAsync<ObservableCollection<ItemsForComboBox>>("Transactions/ProductItems", TransactionId);

        public async Task<ObservableCollection<ProductUnitModel>> ReloadProductUnits() => await Processor.GetCollectionAsync<ObservableCollection<ProductUnitModel>>("Invoices/ProductUnits", null);

        public async Task<ObservableCollection<ItemsForComboBox>> ReloadTransactionNames(int Id = 0) => await Processor.GetCollectionAsync<ObservableCollection<ItemsForComboBox>>("Transactions/TransactionNames", Id == 0 ? null : Id.ToString());

        public async Task<ObservableCollection<string>> ReloadBankNames() => await Processor.GetCollectionAsync<ObservableCollection<string>>("Cheque/Banknames", null);

        public async Task<ObservableCollection<ItemsForComboBox>> ReloadCustomerNames() => await Processor.GetCollectionAsync<ObservableCollection<ItemsForComboBox>>("Invoices/CustomerNames", null);
    }
}