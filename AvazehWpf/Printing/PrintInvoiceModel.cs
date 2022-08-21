using SharedLibrary.DalModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvazehWpf.Printing
{
    internal class PrintInvoiceModel
    {
        private InvoiceModel _invoice;
        private bool _printDate;
        private bool _printInvoiceDescription;
        private bool _printCustomerDescription;
        private bool _printUserDescription;
        private string _userDescription;
        private string _footerText;
        private string _mainHeaderText;
        private string _headerDescription;
        private string _invoiceType;
        private Image _logo;
        private bool _hasAttachment;

        public InvoiceModel Invoice
        {
            get => _invoice;
            set => _invoice = value;
        }

        public bool PrintDate
        {
            get => _printDate;
            set => _printDate = value;
        }

        public bool PrintInvoiceDescription
        {
            get => _printInvoiceDescription;
            set => _printInvoiceDescription = value;
        }

        public bool PrintCustomerDescription
        {
            get => _printCustomerDescription;
            set => _printCustomerDescription = value;
        }

        public bool PrintUserDescription
        {
            get => _printUserDescription;
            set => _printUserDescription = value;
        }

        public string UserDescription
        {
            get => _userDescription;
            set => _userDescription = value;
        }

        public string InvoiceDescription
        {
            get => Invoice.Descriptions;
            set => Invoice.Descriptions = value;
        }

        public string CustomerDescription
        {
            get => Invoice.Customer.Descriptions;
            set => Invoice.Customer.Descriptions = value;
        }

        public string FooterText //Eg: گردآورنده نرم افزار: صفا دانا
        {
            get => _footerText;
            set => _footerText = value;
        }

        public string MainHeaderText //فروشگاه آوازه
        {
            get => _mainHeaderText;
            set => _mainHeaderText = value;
        }

        public string HeaderDescription //کرکره برقی، جک پارکینگی 01734421122
        {
            get => _headerDescription;
            set => _headerDescription = value;
        }

        public string InvoiceType //پیش فاکتور/فاکتور فروش
        {
            get => _invoiceType;
            set => _invoiceType = value;
        }

        public Image Logo
        {
            get => _logo;
            set => _logo = value;
        }

        public bool HasAttachment //پیوست دارد/ندارد
        {
            get => _hasAttachment;
            set => _hasAttachment = value;
        }
    }
}