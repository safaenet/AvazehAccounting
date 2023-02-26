using AvazehApiClient.DataAccess.Interfaces;
using FluentValidation.Results;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Validators;
using System.Threading.Tasks;

namespace AvazehApiClient.DataAccess.CollectionManagers;

public partial class InvoiceDetailManager : IInvoiceDetailManager
{
    private const string KeyPayment = "InvoicePayment";

    public async Task<InvoicePaymentModel> GetPaymentById(int Id)
    {
        return await ApiProcessor.GetItemAsync<InvoicePaymentModel>(KeyPayment, Id.ToString());
    }

    public async Task<InvoicePaymentModel> CreatePaymentAsync(InvoicePaymentModel item)
    {
        if (item == null || !ValidateItem(item).IsValid) return null;
        var newItem = item.AsDto();
        return await ApiProcessor.CreateItemAsync<InvoicePaymentModel_DTO_Create_Update, InvoicePaymentModel>(KeyPayment, newItem);
    }

    public async Task<InvoicePaymentModel> UpdatePaymentAsync(InvoicePaymentModel item)
    {
        if (item == null || !ValidateItem(item).IsValid) return null;
        var newItem = item.AsDto();
        var result = await ApiProcessor.UpdateItemAsync<InvoicePaymentModel_DTO_Create_Update, InvoicePaymentModel>(KeyPayment, item.Id, newItem);
        result.DateCreated = item.DateCreated;
        return result;
    }

    public async Task<bool> DeletePaymentAsync(int Id)
    {
        if (await ApiProcessor.DeleteItemAsync(KeyPayment, Id))
            return true;
        return false;
    }

    public ValidationResult ValidateItem(InvoicePaymentModel item)
    {
        InvoicePaymentValidator validator = new();
        var result = validator.Validate(item);
        return result;
    }
}