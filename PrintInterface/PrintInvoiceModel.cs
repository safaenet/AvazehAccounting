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
        private List<UserDescriptionModel> userDescriptions;
        private string footerTextLeft;
        private string footerTextRight;
        private string mainHeaderText;
        private string headerDescription1;
        private string headerDescription2;
        private string leftImagePath;
        private string rightImagePath;
        private double totalItemsSellSum;
        private double totalDiscountAmount;
        private double totalInvoiceSum;
        private double totalPayments;
        private double customerPreviousBalance;
        private double totalBalance;
        private string invoiceFinStatus;
        private int invoiceType;

        private int mainHeaderTextFontSize = 30;
        private int headerDescriptionFontSize = 10;
        private int invoiceTypeTextFontSize = 16;
        private int pageHeaderFontSize = 10;
        private int detailsFontSize = 10;
        private int pageFooterFontSize = 10;
        private int descriptionFontSize = 14;

        private string printLayout; //Landscape, Portrait
        private string paperSize; //A5, A4

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
        public List<UserDescriptionModel> UserDescriptions { get => userDescriptions; set => userDescriptions = value; }
        public string FooterTextLeft { get => footerTextLeft; set => footerTextLeft = value; }
        public string FooterTextRight { get => footerTextRight; set => footerTextRight = value; }
        public string MainHeaderText { get => mainHeaderText; set => mainHeaderText = value; }
        public string HeaderDescription1 { get => headerDescription1; set => headerDescription1 = value; }
        public string HeaderDescription2 { get => headerDescription2; set => headerDescription2 = value; }
        public double TotalItemsSellSum { get => totalItemsSellSum; set => totalItemsSellSum = value; }
        public double TotalDiscountAmount { get => totalDiscountAmount; set => totalDiscountAmount = value; }
        public double TotalInvoiceSum { get => totalInvoiceSum; set => totalInvoiceSum = value; }
        public double TotalPayments { get => totalPayments; set => totalPayments = value; }
        public double CustomerPreviousBalance { get => customerPreviousBalance; set => customerPreviousBalance = value; }
        public double TotalBalance { get => totalBalance; set => totalBalance = value; }
        public string InvoiceFinStatus { get => invoiceFinStatus; set => invoiceFinStatus = value; }
        public int InvoiceType { get => invoiceType; set => invoiceType = value; }
        public string LeftImagePath { get => leftImagePath; set => leftImagePath = value; }
        public string RightImagePath { get => rightImagePath; set => rightImagePath = value; }
        public int MainHeaderTextFontSize { get => mainHeaderTextFontSize; set => mainHeaderTextFontSize = value; }
        public int HeaderDescriptionFontSize { get => headerDescriptionFontSize; set => headerDescriptionFontSize = value; }
        public int InvoiceTypeTextFontSize { get => invoiceTypeTextFontSize; set => invoiceTypeTextFontSize = value; }
        public int PageHeaderFontSize { get => pageHeaderFontSize; set => pageHeaderFontSize = value; }
        public int DetailsFontSize { get => detailsFontSize; set => detailsFontSize = value; }
        public int PageFooterFontSize { get => pageFooterFontSize; set => pageFooterFontSize = value; }
        public int DescriptionFontSize { get => descriptionFontSize; set => descriptionFontSize = value; }
        public string PrintLayout { get => printLayout; set => printLayout = value; }
        public string PaperSize { get => paperSize; set => paperSize = value; }
        public bool PrintCustomerPhoneNumber { get => printCustomerPhoneNumber; set => printCustomerPhoneNumber = value; }
        public bool PrintInvoiceId { get => printInvoiceId; set => printInvoiceId = value; }
        public bool PrintCustomerPostAddress { get => printCustomerPostAddress; set => printCustomerPostAddress = value; }
        public string CustomerPostAddress { get => customerPostAddress; set => customerPostAddress = value; }
    }
}