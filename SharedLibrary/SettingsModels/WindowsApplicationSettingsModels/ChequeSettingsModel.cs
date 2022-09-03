using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.SettingsModels.WindowsApplicationSettingsModels
{
    public class ChequeSettingsModel
    {
        public int DataGridFontSize { get; set; } = 12;
        public int PageSize { get; set; } = 50;
        public OrderType QueryOrderType { get; set; } = OrderType.DESC;

        public string NewItemColor { get; set; }
        public string SoldItemColor { get; set; }
        public string NonSufficientFundItemColor { get; set; }
        public string CashedItemColor { get; set; }

        public bool NotifyCloseCheques { get; set; } = true;
        public int NotifyDays { get; set; } = 2;
        public string NotificationColor { get; set; }
    }
}