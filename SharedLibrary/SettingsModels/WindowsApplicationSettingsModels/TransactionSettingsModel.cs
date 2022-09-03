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

        public string NewItemColor { get; set; }
        public string UpdatedItemColor { get; set; }
        public string BalancedItemColor { get; set; }
        public string PositiveItemColor { get; set; }
        public string NegativeItemColor { get; set; }

        public int DetailDataGridFontSize { get; set; } = 12;
        public string DetailNewItemColor { get; set; }
        public string DetailUpdatedItemColor { get; set; }
    }
}