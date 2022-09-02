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
        public int DataGridFontSize { get; set; }
        public int PageSize { get; set; }
        public OrderType QueryOrderType { get; set; }

        public string NewItemColor { get; set; }
        public string SoldItemColor { get; set; }
        public string NonSufficientFundItemColor { get; set; }
        public string CashedItemColor { get; set; }

        public bool NotifyCloseCheques { get; set; }
        public int NotifyDays { get; set; }
        public string NotificationColor { get; set; }
    }
}