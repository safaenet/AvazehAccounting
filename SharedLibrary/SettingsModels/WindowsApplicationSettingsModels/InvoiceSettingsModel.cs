using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.SettingsModels.WindowsApplicationSettingsModels
{
    public class InvoiceSettingsModel
    {
        public int DataGridFontSize { get; set; }
        public int PageSize { get; set; }
        public OrderType QueryOrderType { get; set; }


        public string NewItemColor { get; set; }
        public string UpdatedItemColor { get; set; }
        public string BalancedItemColor { get; set; }
        public string DeptorItemColor { get; set; }
        public string CreditorItemColor { get; set; }

        public string InactiveItemColor { get; set; }
        public string ArchiveItemColor { get; set; }
        public string DeletedItemColor { get; set; }

        public int DetailDataGridFontSize { get; set; }
        public string DetailNewItemColor { get; set; }
        public string DetailUpdatedItemColor { get; set; }
        public bool DetailShowNetProfit { get; set; }
        public bool EnableBarcodeReader { get; set; }
        public int BarcodeAddItemCount { get; set; }
        public bool AutoAddNewProducts { get; set; }
        public bool CanHaveSimilarItems { get; set; }
    }
}