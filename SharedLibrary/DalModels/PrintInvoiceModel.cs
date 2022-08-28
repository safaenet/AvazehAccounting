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
        public bool PrintDate;
        public bool PrintInvoiceDescription;
        public bool PrintCustomerDescription;
        public bool PrintUserDescription;
        public string InvoiceDescription;
        public string CustomerDescription;
        public string UserDescription;
        public string FooterTextLeft;
        public string FooterTextRight;
        public string MainHeaderText;
        public string HeaderDescription1;
        public string HeaderDescription2;
        public string HeaderInvoiceType;
        public bool HasAttachment;
        public double TotalItemsSellSum;
        public double TotalDiscountAmount;
        public double TotalInvoiceSum;
        public double TotalPayments;
        public double CustomerPreviousBalance;
        public double TotalBalance;
        public string InvoiceFinStatus;
        public int InvoiceType;
    }
}