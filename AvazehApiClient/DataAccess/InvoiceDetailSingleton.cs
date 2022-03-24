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
    public class InvoiceDetailSingleton
    {
        public InvoiceDetailSingleton(IApiProcessor processor)
        {
            Processor = processor;
            ReloadProductItems().ConfigureAwait(true);
            ReloadProductUnits().ConfigureAwait(true);
        }

        private readonly IApiProcessor Processor;
        public ObservableCollection<ProductNamesForComboBox> ProductItemsForCombobox { get; set; }
        public ObservableCollection<ProductUnitModel> ProductUnits { get; set; }

        public async Task ReloadProductItems()
        {
            ProductItemsForCombobox = await Processor.GetCollectionAsync<ObservableCollection<ProductNamesForComboBox>>("Invoices/ProductItems", null);
        }

        public async Task ReloadProductUnits()
        {
            ProductUnits = await Processor.GetCollectionAsync<ObservableCollection<ProductUnitModel>>("Invoices/ProductUnits", null);
        }
    }
}