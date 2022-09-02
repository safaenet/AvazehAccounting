using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.SettingsModels.WindowsApplicationSettingsModels
{
    public class AppSettingsModel
    {
        public AppSettingsModel()
        {
            InvoiceSettings = new();
            TransactionSettings = new();
            ChequeSettings = new();
            GeneralSettings = new();
            InvoicePrintSettings = new();
        }
        public GeneralSettingsModel GeneralSettings { get; set; }
        public InvoiceSettingsModel InvoiceSettings { get; set; }
        public InvoicePrintSettingsModel InvoicePrintSettings { get; set; }
        public TransactionSettingsModel TransactionSettings { get; set; }
        public ChequeSettingsModel ChequeSettings { get; set; }
    }
}