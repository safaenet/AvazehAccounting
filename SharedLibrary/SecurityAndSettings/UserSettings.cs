using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.SecurityAndSettings
{
    public class UserSettings
    {
        public string ColorNewItem { get; set; }
        public string ColorSoldItemColor { get; set; }
        public string ColorNonSufficientFundItem { get; set; }
        public string ColorCashedItem { get; set; }
        public string ColorChequeNotification { get; set; }
        public string ColorUpdatedItem { get; set; }
        public string ColorBalancedItem { get; set; }
        public string ColorDeptorItem { get; set; }
        public string ColorCreditorItem { get; set; }
        public string ColorInactiveItem { get; set; }
        public string ColorArchiveItem { get; set; }
        public string ColorDeletedItem { get; set; }
        public string ColorNegativeProfit { get; set; }
        public string ColorPositiveItem { get; set; }
        public string ColorNegativeItem { get; set; }
        public int DataGridFontSize { get; set; }
        public int ChequeListPageSize { get; set; }
        public OrderType ChequeListQueryOrderType { get; set; }
        public int ChequeNotifyDays { get; set; }
        public bool ChequeNotify { get; set; }
        public int InvoicePageSize { get; set; }
        public OrderType InvoiceListQueryOrderType { get; set; }
        public OrderType InvoiceDetailQueryOrderType { get; set; }
        public int TransactionPageSize { get; set; }
        public OrderType TransactionListQueryOrderType { get; set; }
        public bool AutoSelectPersianLanguage { get; set; }
        public int TransactionShortcut1Id { get; set; }
        public int TransactionShortcut2Id { get; set; }
        public int TransactionShortcut3Id { get; set; }
        public string TransactionShortcut1Name { get; set; }
        public string TransactionShortcut2Name { get; set; }
        public string TransactionShortcut3Name { get; set; }
        public bool AskToAddNotExistingProduct { get; set; }
        public bool CanViewNetProfits { get; set; }
        public bool CanUseBarcodeReader { get; set; }
    }
}