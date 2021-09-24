using System;
using System.Linq;
using FluentValidation.Results;
using AvazehWpfApiClient.Models;
using System.Threading.Tasks;
using AvazehWpfApiClient.Models.Validators;

namespace AvazehWpfApiClient.DataAccess.CollectionManagers
{
    public partial class InvoiceCollectionManagerAsync
    {
        private const string KeySub = "InvoiceItem";

        public InvoiceItemModel GetSubItemFromCollectionById(int Id)
        {
            return Items.SingleOrDefault(i => i.Id == Id);
        }

        public async Task<InvoiceItemModel> GetSubItemById(int Id)
        {
            return await ApiProcessor.GetItemAsync<InvoiceItemModel>(Key, Id);
        }

        public async Task<InvoiceItemModel> CreateSubItemAsync(InvoiceItemModel item)
        {
            if (item == null || !ValidateSubItem(item).IsValid) return null;
            var newItem = item.AsDto();
            return await ApiProcessor.CreateItemAsync<InvoiceModel_DTO_Create_Update, InvoiceModel>(Key, newItem);
        }

        public async Task<InvoiceItemModel> UpdateSubItemAsync(InvoiceItemModel item)
        {
            if (item == null || !ValidateSubItem(item).IsValid) return null;
            var newItem = item.AsDto();
            return await ApiProcessor.UpdateItemAsync<InvoiceModel_DTO_Create_Update, InvoiceItemModel>(Key, item.Id, newItem);
        }

        public async Task<bool> DeleteSubItemAsync(int Id)
        {
            if (await ApiProcessor.DeleteItemAsync(Key, Id))
            {
                Items.Remove(GetSubItemFromCollectionById(Id));
                return true;
            }
            return false;
        }

        public ValidationResult ValidateSubItem(InvoiceItemModel item)
        {
            InvoiceItemValidator validator = new();
            var result = validator.Validate(item);
            return result;
        }

        public double GetTotalOrRestTotalBalanceOfCustomer(int CustomerId, int InvoiceId = 0)
        {
            throw new NotImplementedException();
        }
    }
}