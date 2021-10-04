using FluentValidation.Results;
using System.Threading.Tasks;
using AvazehApiClient.DataAccess.Interfaces;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Validators;

namespace AvazehApiClient.DataAccess.CollectionManagers
{
    public partial class InvoiceDetailManager : IInvoiceDetailManager
    {
        public InvoiceDetailManager(IApiProcessor apiProcessor)
        {
            ApiProcessor = apiProcessor;
        }
        private const string KeyItem = "InvoiceItem";
        private IApiProcessor ApiProcessor { get; init; }
        public long CustomerTotalBalance { get; private set; }

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
                return true;
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