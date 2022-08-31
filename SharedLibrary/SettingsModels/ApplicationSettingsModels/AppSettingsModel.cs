using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.SettingsModels.ApplicationSettingsModels
{
    public class AppSettingsModel
    {
        public InvoiceListSettingsModel InvoiceListSettings { get; set; }
        public InvoiceDetailSettingsModel InvoiceDetailSettings { get; set; }
        public InvoicePrintSettingsModel InvoicePrintSettings { get; set; }
    }
}