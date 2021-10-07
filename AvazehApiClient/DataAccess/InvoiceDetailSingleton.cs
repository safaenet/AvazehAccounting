using AvazehApiClient.DataAccess.Interfaces;
using SharedLibrary.DtoModels;
using System;
using System.Collections.Generic;
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
        }

        private readonly IApiProcessor Processor;
        public List<ProductNamesForComboBox> ProductItemsForCombobox { get; set; }

        public async Task ReloadProductItems()
        {
            ProductItemsForCombobox = await Processor.GetCollectionAsync<List<ProductNamesForComboBox>>("Invoices/ProductItems", null);
        }
    }
}