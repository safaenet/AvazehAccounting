using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.SettingsModels.WindowsApplicationSettingsModels
{
    public class GeneralSettingsModel
    {
        public string CompanyName { get; set; }
        public bool AutoSelectPersianLanguage { get; set; }
        public bool ShowTransactionShortcut1 { get; set; }
        public bool ShowTransactionShortcut2 { get; set; }
        public bool ShowTransactionShortcut3 { get; set; }
        public TransactionShortcutModel TransactionShortcut1 { get; set; }
        public TransactionShortcutModel TransactionShortcut2 { get; set; }
        public TransactionShortcutModel TransactionShortcut3 { get; set; }

        public bool CanAddNewInvoice { get; set; }
        public bool CanViewInvoices { get; set; }
        public bool CanEditInvoices { get; set; }

        public bool CanAddNewTransaction { get; set; }
        public bool CanViewTransactions { get; set; }
        public bool CanEditTransactions { get; set; }

        public bool CanAddNewCheque { get; set; }
        public bool CanViewCheques { get; set; }
        public bool CanEditCheques { get; set; }

        public bool CanViewProducts { get; set; }
        public bool CanViewCustomers { get; set; }
    }

    public class TransactionShortcutModel
    {
        public int TransactionId { get; set; }
        public string ShortcutName { get; set; }
    }
}