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

        public string NewItemColor { get; set; } = "#FFF5F533";
        public string SoldItemColor { get; set; } = "#ff66e6ff";
        public string NonSufficientFundItemColor { get; set; } = "#ffff9c7a";
        public string CashedItemColor { get; set; } = "#ff94ffb6";

        public bool NotifyCloseCheques { get; set; } = true;
        public int NotifyDays { get; set; } = 2;
        public string NotificationColor { get; set; } = "#fff4ff8c";
    }
}