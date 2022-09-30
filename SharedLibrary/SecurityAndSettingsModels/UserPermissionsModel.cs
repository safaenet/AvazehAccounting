using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.SecurityAndSettingsModels
{
    public class UserPermissionsModel
    {
        public bool CanViewCustomersList { get; set; } = true;
        public bool CanViewCustomerDetails { get; set; } = true;
        public bool CanViewProductsList { get; set; } = true;
        public bool CanViewProductDetails { get; set; } = true;
        public bool CanViewInvoicesList { get; set; } = true;
        public bool CanViewInvoiceDetails { get; set; } = true;
        public bool CanViewTransactionsList { get; set; } = true;
        public bool CanViewTransactionDetails { get; set; } = true;
        public bool CanViewChequesList { get; set; } = true;
        public bool CanViewChequeDetails { get; set; } = true;

        public bool CanAddNewCustomer { get; set; } = true;
        public bool CanAddNewProduct { get; set; } = true;
        public bool CanAddNewInvoice { get; set; } = true;
        public bool CanAddNewTransaction { get; set; } = true;
        public bool CanAddNewCheque { get; set; } = true;

        public bool CanEditCustomer { get; set; } = true;
        public bool CanEditProduct { get; set; } = true;
        public bool CanEditInvoice { get; set; } = true;
        public bool CanEditTransaction { get; set; } = true;
        public bool CanEditCheque { get; set; } = true;

        public bool CanDeleteCustomer { get; set; } = true;
        public bool CanDeleteProduct { get; set; } = true;
        public bool CanDeleteInvoice { get; set; } = true;
        public bool CanDeleteInvoiceItem { get; set; } = true;
        public bool CanDeleteTransaction { get; set; } = true;
        public bool CanDeleteTransactionItem { get; set; } = true;
        public bool CanDeleteCheque { get; set; } = true;

        public bool CanPrintInvoice { get; set; } = true;
        public bool CanPrintTransaction { get; set; } = true;
        public bool CanViewNetProfits { get; set; } = true;
        public bool CanUseBarcodeReader { get; set; } = true;

        public bool CanManageItself { get; set; } = true;
        public bool CanManageOthers { get; set; } = true;
    }
}