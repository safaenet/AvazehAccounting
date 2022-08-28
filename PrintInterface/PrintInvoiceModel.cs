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
        private bool printDate;
        private bool printInvoiceDescription;
        private bool printCustomerDescription;
        private bool printUserDescription;
        private string invoiceDescription;
        private string customerDescription;
        private string userDescription;
        private string footerTextLeft;
        private string footerTextRight;
        private string mainHeaderText;
        private string headerDescription1;
        private string headerDescription2;
        private string headerInvoiceType;
        private bool hasAttachment;
        private double totalItemsSellSum;
        private double totalDiscountAmount;
        private double totalInvoiceSum;
        private double totalPayments;
        private double customerPreviousBalance;
        private double totalBalance;
        private string invoiceFinStatus;
        private int invoiceType;

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
        public string UserDescription { get => userDescription; set => userDescription = value; }
        public string FooterTextLeft { get => footerTextLeft; set => footerTextLeft = value; }
        public string FooterTextRight { get => footerTextRight; set => footerTextRight = value; }
        public string MainHeaderText { get => mainHeaderText; set => mainHeaderText = value; }
        public string HeaderDescription1 { get => headerDescription1; set => headerDescription1 = value; }
        public string HeaderDescription2 { get => headerDescription2; set => headerDescription2 = value; }
        public string HeaderInvoiceType { get => headerInvoiceType; set => headerInvoiceType = value; }
        public bool HasAttachment { get => hasAttachment; set => hasAttachment = value; }
        public double TotalItemsSellSum { get => totalItemsSellSum; set => totalItemsSellSum = value; }
        public double TotalDiscountAmount { get => totalDiscountAmount; set => totalDiscountAmount = value; }
        public double TotalInvoiceSum { get => totalInvoiceSum; set => totalInvoiceSum = value; }
        public double TotalPayments { get => totalPayments; set => totalPayments = value; }
        public double CustomerPreviousBalance { get => customerPreviousBalance; set => customerPreviousBalance = value; }
        public double TotalBalance { get => totalBalance; set => totalBalance = value; }
        public string InvoiceFinStatus { get => invoiceFinStatus; set => invoiceFinStatus = value; }
        public int InvoiceType { get => invoiceType; set => invoiceType = value; }
    }
}