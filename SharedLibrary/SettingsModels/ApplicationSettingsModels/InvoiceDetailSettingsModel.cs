using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.SettingsModels.ApplicationSettingsModels
{
    public class InvoiceDetailSettingsModel
    {
        public int DataGridFontSize { get; set; }
        public string NewItemColor { get; set; }
        public string UpdatedItemColor { get; set; }
        public bool ShowNetProfit { get; set; }
    }
}