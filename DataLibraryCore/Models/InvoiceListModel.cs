using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibraryCore.Models
{
    /// <summary>
    /// This model is for viewing Invoices in ListView
    /// </summary>
    public class InvoiceListModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerFullName { get; set; }
        public string DateCreated { get; set; }
        public string DateUpdated { get; set; }
        public double TotalInvoiceSum { get; set; }
        public double TotalPayments { get; set; }
        public InvoiceLifeStatus LifeStatus { get; set; }
        public double TotalBalance => TotalInvoiceSum - TotalPayments;
        public InvoiceFinancialStatus InvoiceFinancialStatus => TotalBalance == 0 ? InvoiceFinancialStatus.Balanced : TotalBalance > 0 ? InvoiceFinancialStatus.Deptor : InvoiceFinancialStatus.Creditor;
    }
}