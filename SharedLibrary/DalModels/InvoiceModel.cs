using SharedLibrary.Enums;
using System.Collections.ObjectModel;
using System.Linq;

namespace SharedLibrary.DalModels
{
    public class InvoiceModel
    {
        public int Id { get; set; }
        public CustomerModel Customer { get; set; }
        public string DateCreated { get; set; }
        public string TimeCreated { get; set; }
        public string DateUpdated { get; set; }
        public string TimeUpdated { get; set; }
        public ObservableCollection<InvoiceItemModel> Items { get; set; }
        public ObservableCollection<InvoicePaymentModel> Payments { get; set; }
        public DiscountTypes DiscountType { get; set; }
        public double DiscountValue { get; set; }
        public string Descriptions { get; set; }
        public InvoiceLifeStatus LifeStatus { get; set; }
        public double TotalItemsBuySum => Items == null || Items.Count == 0 ? 0 : Items.Sum(i => i.TotalBuyValue);
        public double TotalItemsSellSum => Items == null || Items.Count == 0 ? 0 : Items.Sum(i => i.TotalSellValue);
        public double TotalDiscountAmount => DiscountType == DiscountTypes.Percent ? TotalItemsSellSum * DiscountValue : DiscountValue;
        public double TotalInvoiceSum => TotalItemsSellSum - TotalDiscountAmount;
        public double TotalPayments => Payments == null || Payments.Count == 0 ? 0 : Payments.Sum(i => i.PayAmount);
        public double TotalBalance => TotalInvoiceSum - TotalPayments;
        public double NetProfit => TotalInvoiceSum - TotalItemsBuySum;
        public double CurrentProfit => NetProfit - TotalBalance;
        public InvoiceFinancialStatus InvoiceFinancialStatus => TotalBalance == 0 ? InvoiceFinancialStatus.Balanced : TotalBalance > 0 ? InvoiceFinancialStatus.Deptor : InvoiceFinancialStatus.Creditor;
    }
}