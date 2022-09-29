using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.SecurityAndSettingsModels
{
    public class GeneralSettings
    {
        public string CompanyName { get; set; } = "آوازه";
        public int BarcodeAddItemCount { get; set; } = 1;
        public bool CanHaveDuplicateItemsInInvoice { get; set; } = true;
    }
}