using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintInterface
{
    public class PrintInvoiceModel
    {
        private int invoiceId;
        private List<InvoiceItemForPrintModel> products;
        private int customerId;
        private string customerFullName;
        private string customerPhoneNumber;
        private string invoiceDateCreated;
        private bool printInvoiceId;
        private bool printDate;
        private bool printCustomerPhoneNumber;
        private bool printCustomerPostAddress;
        private string customerPostAddress;
        private bool printInvoiceDescription;
        private bool printCustomerDescription;
        private bool printUserDescription;
        private string invoiceDescription;
        private string customerDescription;
        
        private decimal totalItemsSellSum;
        private decimal totalDiscountAmount;
        private decimal totalInvoiceSum;
        private decimal totalPayments;
        private decimal customerPreviousBalance;
        private decimal totalBalance;
        private string invoiceFinStatus;
        private int invoiceType;
        private PrintSettingsModel printSettings;

        public int InvoiceId { get => invoiceId; set => invoiceId = value; }
        public List<InvoiceItemForPrintModel> Products { get => products; set => products = value; }
        public int CustomerId { get => customerId; set => customerId = value; }
        public string CustomerFullName { get => customerFullName; set => customerFullName = value; }
        public string CustomerPhoneNumber { get => customerPhoneNumber; set => customerPhoneNumber = value; }
        public string InvoiceDateCreated { get => invoiceDateCreated; set => invoiceDateCreated = value; }
        public bool PrintDate { get => printDate; set => printDate = value; }
        public bool PrintInvoiceDescription { get => printInvoiceDescription; set => printInvoiceDescription = value; }
        public bool PrintCustomerDescription { get => printCustomerDescription; set => printCustomerDescription = value; }
        public bool PrintUserDescription { get => printUserDescription; set => printUserDescription = value; }
        public string InvoiceDescription { get => invoiceDescription; set => invoiceDescription = value; }
        public string CustomerDescription { get => customerDescription; set => customerDescription = value; }
        public decimal TotalItemsSellSum { get => totalItemsSellSum; set => totalItemsSellSum = value; }
        public decimal TotalDiscountAmount { get => totalDiscountAmount; set => totalDiscountAmount = value; }
        public decimal TotalInvoiceSum { get => totalInvoiceSum; set => totalInvoiceSum = value; }
        public decimal TotalPayments { get => totalPayments; set => totalPayments = value; }
        public decimal CustomerPreviousBalance { get => customerPreviousBalance; set => customerPreviousBalance = value; }
        public decimal TotalBalance { get => totalBalance; set => totalBalance = value; }
        public string InvoiceFinStatus { get => invoiceFinStatus; set => invoiceFinStatus = value; }
        public int InvoiceType { get => invoiceType; set => invoiceType = value; }
        public bool PrintCustomerPhoneNumber { get => printCustomerPhoneNumber; set => printCustomerPhoneNumber = value; }
        public bool PrintInvoiceId { get => printInvoiceId; set => printInvoiceId = value; }
        public bool PrintCustomerPostAddress { get => printCustomerPostAddress; set => printCustomerPostAddress = value; }
        public string CustomerPostAddress { get => customerPostAddress; set => customerPostAddress = value; }
        public PrintSettingsModel PrintSettings { get => printSettings; set => printSettings = value; }
    }
}