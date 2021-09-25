using System;
using System.Linq;
using FluentValidation.Results;
using AvazehApiClient.Models;
using System.Threading.Tasks;
using AvazehApiClient.Models.Validators;
using AvazehApiClient.DataAccess.Interfaces;
using System.Collections.ObjectModel;

namespace AvazehApiClient.DataAccess.CollectionManagers
{
    public partial class InvoiceManagerAsync
    {
        public InvoiceManagerAsync(IApiProcessor apiProcessor, IInvoiceCollectionManagerAsync collectionManager)
        {
            ApiProcessor = apiProcessor;
            CollectionManager = collectionManager;
        }
        private const string KeyItem = "InvoiceItem";
        private IApiProcessor ApiProcessor { get; init; }
        public IInvoiceCollectionManagerAsync CollectionManager { get; init; }
        public InvoiceModel Invoice { get; set; }
        public long CustomerTotalBalance { get; private set; }

        public InvoiceItemModel GetItemFromCollectionById(int Id)
        {
            return Invoice.Items.SingleOrDefault(i => i.Id == Id);
        }

        public async Task<InvoiceItemModel> GetItemById(int Id)
        {
            return await ApiProcessor.GetItemAsync<InvoiceItemModel>(KeyItem, Id);
        }

        public async Task<InvoiceItemModel> CreateItemAsync(InvoiceItemModel item)
        {
            if (item == null || !ValidateItem(item).IsValid) return null;
            var newItem = item.AsDto();
            return await ApiProcessor.CreateItemAsync<InvoiceItemModel_DTO_Create_Update, InvoiceItemModel>(KeyItem, newItem);
        }

        public async Task<InvoiceItemModel> UpdateItemAsync(InvoiceItemModel item)
        {
            if (item == null || !ValidateItem(item).IsValid) return null;
            var newItem = item.AsDto();
            return await ApiProcessor.UpdateItemAsync<InvoiceItemModel_DTO_Create_Update, InvoiceItemModel>(KeyItem, item.Id, newItem);
        }

        public async Task<bool> DeleteItemAsync(int Id)
        {
            if (await ApiProcessor.DeleteItemAsync(KeyItem, Id))
            {
                Invoice.Items.Remove(GetItemFromCollectionById(Id));
                return true;
            }
            return false;
        }

        public ValidationResult ValidateItem(InvoiceItemModel item)
        {
            InvoiceItemValidator validator = new();
            var result = validator.Validate(item);
            return result;
        }
    }
}