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
        public int DataGridFontSize { get; set; } = 12;
        public int PageSize { get; set; } = 50;
        public OrderType QueryOrderType { get; set; } = OrderType.DESC;


        public string NewItemColor { get; set; }
        public string UpdatedItemColor { get; set; }
        public string BalancedItemColor { get; set; }
        public string DeptorItemColor { get; set; }
        public string CreditorItemColor { get; set; }

        public string InactiveItemColor { get; set; }
        public string ArchiveItemColor { get; set; }
        public string DeletedItemColor { get; set; }

        public int DetailDataGridFontSize { get; set; } = 12;
        public string DetailNewItemColor { get; set; }
        public string DetailUpdatedItemColor { get; set; }
        public bool DetailShowNetProfit { get; set; } = true;
        public bool EnableBarcodeReader { get; set; } = true;
        public int BarcodeAddItemCount { get; set; } = 1;
        public bool AutoAddNewProducts { get; set; } = true;
        public bool CanHaveSimilarItems { get; set; } = true;
    }
}