using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.SecurityAndSettings
{
    public class UserPermissions
    {
        public bool CanViewCustomers { get; set; }
        public bool CanViewProducts { get; set; }
        public bool CanViewInvoicesList { get; set; }
        public bool CanViewInvoiceDetails { get; set; }
        public bool CanViewTransactionsList { get; set; }
        public bool CanViewTransactionDetails { get; set; }
        public bool CanViewCheques { get; set; }
        public bool CanAddNewCustomer { get; set; }
        public bool CanAddNewProduct { get; set; }
        public bool CanAddNewInvoice { get; set; }
        public bool CanAddNewTransaction { get; set; }
        public bool CanAddNewCheque { get; set; }
        public bool CanEditCustomers { get; set; }
        public bool CanEditProducts { get; set; }
        public bool CanEditInvoices { get; set; }
        public bool CanEditTransactions { get; set; }
        public bool CanEditCheques { get; set; }
        public bool CanDeleteCustomer { get; set; }
        public bool CanDeleteProduct { get; set; }
        public bool CanDeleteInvoice { get; set; }
        public bool CanDeleteInvoiceItem { get; set; }
        public bool CanDeleteTransaction { get; set; }
        public bool CanDeleteTransactionItem { get; set; }
        public bool CanDeleteCheque { get; set; }
        public bool CanPrintInvoice { get; set; }
        public bool CanPrintTransaction { get; set; }
        public bool CanChangeItsSettings { get; set; }
        public bool CanChangeItsPassword { get; set; }
        public bool CanAddUser { get; set; }
        public bool CanEditOtherUsersPermission { get; set; }
        public bool CanEditOtherUsersSettings { get; set; }
    }
}