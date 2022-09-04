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

        public string NewItemColor { get; set; } = "#FFF5F533";
        public string UpdatedItemColor { get; set; } = "#FFDEDEDE";
        public string BalancedItemColor { get; set; } = "#ff94ffb6";
        public string DeptorItemColor { get; set; } = "#ffff9c7a";
        public string CreditorItemColor { get; set; } = "#ff7ad3ff";

        public string InactiveItemColor { get; set; } = "#ffc9c9c9";
        public string ArchiveItemColor { get; set; } = "#ffffe0a3";
        public string DeletedItemColor { get; set; } = "#ffff6b6b";

        public int DetailDataGridFontSize { get; set; } = 12;
        public string DetailNewItemColor { get; set; } = "#FFF5F533";
        public string DetailUpdatedItemColor { get; set; } = "#FFDEDEDE";
        public string DetailNegativeProfitColor { get; set; } = "#ffffadad";
        public bool DetailShowNetProfit { get; set; } = true;
        public bool EnableBarcodeReader { get; set; } = true;
        public int BarcodeAddItemCount { get; set; } = 1;
        public bool AutoAddNewProducts { get; set; } = true;
        public bool CanHaveSimilarItems { get; set; } = true;
    }
}