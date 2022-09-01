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
        public int DataGridFontSize { get; set; }
        public int PageSize { get; set; }
        public string QueryOrderBy { get; set; }
        public OrderType QueryOrderType { get; set; }
        public string NewItemColor { get; set; }
        public string UpdatedItemColor { get; set; }
        public string BalancedItemColor { get; set; }
        public string PositiveItemColor { get; set; }
        public string NegativeItemColor { get; set; }

        public int DetailDataGridFontSize { get; set; }
        public string DetailNewItemColor { get; set; }
        public string DetailUpdatedItemColor { get; set; }
    }
}