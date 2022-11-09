using SharedLibrary.DalModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.SecurityAndSettingsModels
{
    public class GeneralSettingsModel
    {
        public string CompanyName { get; set; } = "آوازه";
        public int BarcodeAddItemCount { get; set; } = 1;
        public bool CanHaveDuplicateItemsInInvoice { get; set; } = true;
        public List<ProductUnitModel> ProductUnits{ get; set; }
    }
}