using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.SecurityAndSettingsModels
{
    public class UserPermissions
    {
        public bool CanViewCustomersList { get; set; }
        public bool CanViewCustomerDetails { get; set; }
        public bool CanViewProductsList { get; set; }
        public bool CanViewProductDetails { get; set; }
        public bool CanViewInvoicesList { get; set; }
        public bool CanViewInvoiceDetails { get; set; }
        public bool CanViewTransactionsList { get; set; }
        public bool CanViewTransactionDetails { get; set; }
        public bool CanViewChequesList { get; set; }
        public bool CanViewChequeDetails { get; set; }
        public bool CanAddNewCustomer { get; set; }
        public bool CanAddNewProduct { get; set; }
        public bool CanAddNewInvoice { get; set; }
        public bool CanAddNewTransaction { get; set; }
        public bool CanAddNewCheque { get; set; }
        public bool CanEditCustomer { get; set; }
        public bool CanEditProduct { get; set; }
        public bool CanEditInvoice { get; set; }
        public bool CanEditTransaction { get; set; }
        public bool CanEditCheque { get; set; }
        public bool CanDeleteCustomer { get; set; }
        public bool CanDeleteProduct { get; set; }
        public bool CanDeleteInvoice { get; set; }
        public bool CanDeleteInvoiceItem { get; set; }
        public bool CanDeleteTransaction { get; set; }
        public bool CanDeleteTransactionItem { get; set; }
        public bool CanDeleteCheque { get; set; }
        public bool CanPrintInvoice { get; set; }
        public bool CanPrintTransaction { get; set; }

        public bool CanManageItself { get; set; }
        public bool CanManageOthers { get; set; }
    }
}