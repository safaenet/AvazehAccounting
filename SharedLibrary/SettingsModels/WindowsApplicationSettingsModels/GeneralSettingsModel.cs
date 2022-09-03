using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.SettingsModels.WindowsApplicationSettingsModels
{
    public class GeneralSettingsModel
    {
        public GeneralSettingsModel()
        {
            TransactionShortcut1 = new();
            TransactionShortcut2 = new();
            TransactionShortcut3 = new();
        }
        public string CompanyName { get; set; } = "آوازه";
        public bool AutoSelectPersianLanguage { get; set; }
        public bool ShowTransactionShortcut1 { get; set; }
        public bool ShowTransactionShortcut2 { get; set; }
        public bool ShowTransactionShortcut3 { get; set; }
        public TransactionShortcutModel TransactionShortcut1 { get; set; }
        public TransactionShortcutModel TransactionShortcut2 { get; set; }
        public TransactionShortcutModel TransactionShortcut3 { get; set; }

        public bool CanAddNewInvoice { get; set; } = true;
        public bool CanViewInvoices { get; set; } = true;
        public bool CanEditInvoices { get; set; } = true;

        public bool CanAddNewTransaction { get; set; } = true;
        public bool CanViewTransactions { get; set; } = true;
        public bool CanEditTransactions { get; set; } = true;

        public bool CanAddNewCheque { get; set; } = true;
        public bool CanViewCheques { get; set; } = true;
        public bool CanEditCheques { get; set; } = true;

        public bool CanViewProducts { get; set; } = true;
        public bool CanViewCustomers { get; set; } = true;
    }

    public class TransactionShortcutModel
    {
        public int TransactionId { get; set; }
        public string ShortcutName { get; set; } = "نام میانبر";
    }
}