using SharedLibrary.Contracts;
using SharedLibrary.Enums;

namespace AvazehWpf.Models
{
    public class InvoiceSummaryExtraDetailsModel : InvoiceSummaryDTO
    {
        public decimal TotalInvoiceBalance => TotalInvoiceSum - TotalPayments;
        public decimal TotalBalance => TotalInvoiceBalance + PrevInvoiceBalance;
        public string InvoiceTitle => string.IsNullOrEmpty(About) ? CustomerFullName : CustomerFullName + " - " + About;
        public string DateTimeCreated => TimeCreated + " " + DateCreated;
        public string DateTimeUpdated => TimeUpdated + " " + DateUpdated;
        public InvoiceFinancialStatus InvoiceFinancialStatus => TotalBalance == 0 ? InvoiceFinancialStatus.Balanced : TotalBalance > 0 ? InvoiceFinancialStatus.Deptor : InvoiceFinancialStatus.Creditor;
    }
}