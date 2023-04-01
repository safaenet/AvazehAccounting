using SharedLibrary.Enums;
using System.Collections.ObjectModel;
using System.Linq;

namespace SharedLibrary.DalModels;

public class InvoiceModel
{
    public int Id { get; set; }
    public CustomerModel Customer { get; set; }
    public string About { get; set; }
    public string DateCreated { get; set; }
    public string TimeCreated { get; set; }
    public string DateUpdated { get; set; }
    public string TimeUpdated { get; set; }
    public ObservableCollection<InvoiceItemModel> Items { get; set; }
    public ObservableCollection<InvoicePaymentModel> Payments { get; set; }
    public DiscountTypes DiscountType { get; set; } = DiscountTypes.Amount;
    public decimal DiscountValue { get; set; }
    public string Descriptions { get; set; }
    public InvoiceLifeStatus LifeStatus { get; set; }
    public int PrevInvoiceId { get; set; }
    public decimal PrevInvoiceBalance { get; set; }
    public decimal TotalItemsBuySum => Items == null || Items.Count == 0 ? 0 : Items.Sum(i => i.TotalBuyValue);
    public decimal TotalItemsSellSum => Items == null || Items.Count == 0 ? 0 : Items.Sum(i => i.TotalSellValue);
    public decimal TotalDiscountAmount => DiscountType == DiscountTypes.Percent ? (TotalItemsSellSum * DiscountValue / 100) : DiscountValue;
    public decimal TotalInvoiceSum => TotalItemsSellSum - TotalDiscountAmount;
    public decimal TotalPayments => Payments == null || Payments.Count == 0 ? 0 : (decimal)Payments.Sum(i => i.PayAmount);
    public decimal TotalInvoiceBalance => TotalInvoiceSum - TotalPayments;
    public decimal TotalBalance => TotalInvoiceBalance + PrevInvoiceBalance;
    public decimal NetProfit => TotalInvoiceSum - TotalItemsBuySum; //(Prev invoice not included)
    public decimal CurrentProfit => NetProfit - TotalInvoiceBalance;
    public InvoiceFinancialStatus InvoiceFinancialStatus => TotalBalance == 0 ? InvoiceFinancialStatus.Balanced : TotalBalance > 0 ? InvoiceFinancialStatus.Deptor : InvoiceFinancialStatus.Creditor;
}