using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.SettingsModels.WindowsApplicationSettingsModels
{
    public class TransactionSettingsModel
    {
        public int DataGridFontSize { get; set; } = 12;
        public int PageSize { get; set; } = 50;
        public OrderType QueryOrderType { get; set; } = OrderType.DESC;

        public string NewItemColor { get; set; } = "#FFF5F533";
        public string UpdatedItemColor { get; set; } = "#FFDEDEDE";

        public string BalancedItemColor { get; set; } = "#ff94ffb6";
        public string PositiveItemColor { get; set; } = "#ff7ad3ff";
        public string NegativeItemColor { get; set; } = "#ffff9c7a";

        public int DetailDataGridFontSize { get; set; } = 12;
        public string DetailNewItemColor { get; set; } = "#FFF5F533";
        public string DetailUpdatedItemColor { get; set; } = "#FFDEDEDE";
        public string DetailPositiveItemColor { get; set; } = "#ff7ad3ff";
        public string DetailNegativeItemColor { get; set; } = "#ffff9c7a";
    }
}