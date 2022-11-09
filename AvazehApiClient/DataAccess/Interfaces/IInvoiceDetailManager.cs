using AvazehApiClient.DataAccess.Interfaces;
using FluentValidation.Results;
using SharedLibrary.DalModels;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AvazehApiClient.DataAccess.Interfaces
{
    public interface IInvoiceDetailManager : IDetailManagerBase<InvoiceItemModel>
    {
        Task<ObservableCollection<RecentSellPriceModel>> GetRecentSellPrices(int MaxRecord, int CustomerId, int ProductId);
        Task<InvoicePaymentModel> GetPaymentById(int Id);
        Task<InvoicePaymentModel> CreatePaymentAsync(InvoicePaymentModel item);
        Task<InvoicePaymentModel> UpdatePaymentAsync(InvoicePaymentModel item);
        Task<bool> DeletePaymentAsync(int Id);
        ValidationResult ValidateItem(InvoicePaymentModel item);
    }
}