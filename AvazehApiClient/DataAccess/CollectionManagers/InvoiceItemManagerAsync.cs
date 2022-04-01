using FluentValidation.Results;
using System.Threading.Tasks;
using AvazehApiClient.DataAccess.Interfaces;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Validators;
using System.Collections.ObjectModel;

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
            var result = await ApiProcessor.UpdateItemAsync<InvoiceItemModel_DTO_Create_Update, InvoiceItemModel>(KeyItem, item.Id, newItem);
            result.DateCreated = item.DateCreated;
            result.TimeCreated = item.TimeCreated;
            return result;
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

        public async Task<ObservableCollection<RecentSellPriceModel>> GetRecentSellPrices(int MaxRecord, int CustomerId, int ProductId)
        {
            return await ApiProcessor.GetCollectionAsync<ObservableCollection<RecentSellPriceModel>>(KeyItem, MaxRecord, CustomerId, ProductId);
        }
    }
}