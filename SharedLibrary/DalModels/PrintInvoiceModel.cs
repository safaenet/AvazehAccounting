using SharedLibrary.DalModels;
using SharedLibrary.SettingsModels;
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
        public string CustomerPostAddress;
        public string CustomerDescription = "";
        public string InvoiceDescription = "";
        public string InvoiceFinStatus;
        public double TotalBalance;
        public double TotalDiscountAmount;
        public double TotalItemsSellSum;
        public double TotalInvoiceSum;
        public double TotalPayments;

        public double CustomerPreviousBalance;
        public int InvoiceType;
        public bool PrintInvoiceDescription;
        public bool PrintCustomerDescription;
        public bool PrintUserDescription;
        public bool PrintCustomerPostAddress;
        public bool PrintInvoiceId = true;
        public bool PrintDate = true;
        public bool PrintCustomerPhoneNumber = true;
        public PrintSettingsModel PrintSettings = new();
    }
}