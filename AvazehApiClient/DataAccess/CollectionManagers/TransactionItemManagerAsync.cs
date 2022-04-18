using FluentValidation.Results;
using System.Threading.Tasks;
using AvazehApiClient.DataAccess.Interfaces;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Validators;
using System.Collections.ObjectModel;

namespace AvazehApiClient.DataAccess.CollectionManagers
{
    public class TransactionDetailManager : ITransactionDetailManager
    {
        public TransactionDetailManager(IApiProcessor apiProcessor)
        {
            ApiProcessor = apiProcessor;
        }
        private const string KeyItem = "TransactionItem";
        private IApiProcessor ApiProcessor { get; init; }

        public async Task<TransactionItemModel> GetItemById(int Id)
        {
            return await ApiProcessor.GetItemAsync<TransactionItemModel>(KeyItem, Id.ToString());
        }

        public async Task<TransactionItemModel> CreateItemAsync(TransactionItemModel item)
        {
            if (item == null || !ValidateItem(item).IsValid) return null;
            var newItem = item.AsDto();
            return await ApiProcessor.CreateItemAsync<TransactionItemModel_DTO_Create_Update, TransactionItemModel>(KeyItem, newItem);
        }

        public async Task<TransactionItemModel> UpdateItemAsync(TransactionItemModel item)
        {
            if (item == null || !ValidateItem(item).IsValid) return null;
            var newItem = item.AsDto();
            var result = await ApiProcessor.UpdateItemAsync<TransactionItemModel_DTO_Create_Update, TransactionItemModel>(KeyItem, item.Id, newItem);
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

        public ValidationResult ValidateItem(TransactionItemModel item)
        {
            TransactionItemValidator validator = new();
            var result = validator.Validate(item);
            return result;
        }
    }
}