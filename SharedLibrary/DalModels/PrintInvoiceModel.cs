using SharedLibrary.DalModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.DalModels
{
    public class PrintInvoiceModel
    {
        public int InvoiceId;
        public List<InvoiceItemForPrintModel> Products;
        public int CustomerId;
        public string CustomerFullName;
        public string CustomerPhoneNumber;
        public string InvoiceDateCreated;
        public bool PrintInvoiceId = true;
        public bool PrintDate = true;
        public bool PrintCustomerPhoneNumber = true;
        public bool PrintInvoiceDescription;
        public bool PrintCustomerDescription;
        public bool PrintUserDescription;
        public string InvoiceDescription = "";
        public string CustomerDescription = "";
        public List<UserDescriptionModel> UserDescriptions;
        public string FooterTextLeft;
        public string FooterTextRight;
        public string MainHeaderText = "";
        public string HeaderDescription1 = "";
        public string HeaderDescription2 = "";
        public string LeftImagePath;
        public string RightImagePath;
        public double TotalItemsSellSum;
        public double TotalDiscountAmount;
        public double TotalInvoiceSum;
        public double TotalPayments;
        public double CustomerPreviousBalance;
        public double TotalBalance;
        public string InvoiceFinStatus;
        public int InvoiceType;

        public int MainHeaderTextFontSize = 30;
        public int HeaderDescriptionFontSize = 10;
        public int InvoiceTypeTextFontSize = 16;
        public int PageHeaderFontSize = 10;
        public int DetailsFontSize = 10;
        public int PageFooterFontSize = 10;
        public int DescriptionFontSize = 14;

        public string PrintLayout = "عمودی"; //Landscape, Portrait
        public string PaperSize = "A5"; //A5, A4
    }
}