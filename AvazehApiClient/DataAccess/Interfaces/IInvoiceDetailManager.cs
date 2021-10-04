using AvazehApiClient.DataAccess.Interfaces;
using FluentValidation.Results;
using SharedLibrary.DalModels;
using System.Threading.Tasks;

namespace AvazehApiClient.DataAccess.Interfaces
{
    public interface IInvoiceDetailManager
    {
        Task<InvoiceItemModel> GetItemById(int Id);
        Task<InvoiceItemModel> CreateItemAsync(InvoiceItemModel item);
        Task<InvoiceItemModel> UpdateItemAsync(InvoiceItemModel item);
        Task<bool> DeleteItemAsync(int Id);
        ValidationResult ValidateItem(InvoiceItemModel item);

        Task<InvoicePaymentModel> GetPaymentById(int Id);
        Task<InvoicePaymentModel> CreatePaymentAsync(InvoicePaymentModel item);
        Task<InvoicePaymentModel> UpdatePaymentAsync(InvoicePaymentModel item);
        Task<bool> DeletePaymentAsync(int Id);
        ValidationResult ValidateItem(InvoicePaymentModel item);
    }
}