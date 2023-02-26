using SharedLibrary.Enums;

namespace SharedLibrary.SecurityAndSettingsModels;

public class UserSettingsModel
{
    public string ColorNewItem { get; set; } = "#FFF5F533";
    public string ColorSoldItem { get; set; } = "#ff66e6ff";
    public string ColorNonSufficientFundItem { get; set; } = "#ffff9c7a";
    public string ColorCashedItem { get; set; } = "#ff94ffb6";
    public string ColorChequeNotification { get; set; } = "#fff4ff8c";
    public string ColorUpdatedItem { get; set; } = "#FFDEDEDE";
    public string ColorBalancedItem { get; set; } = "#ff94ffb6";
    public string ColorDeptorItem { get; set; } = "#ffff9c7a";
    public string ColorCreditorItem { get; set; } = "#ff7ad3ff";
    public string ColorInactiveItem { get; set; } = "#ffc9c9c9";
    public string ColorArchivedItem { get; set; } = "#ffffe0a3";
    public string ColorDeletedItem { get; set; } = "#ffff6b6b";
    public string ColorNegativeProfit { get; set; } = "#ffffadad";
    public string ColorPositiveItem { get; set; } = "#ff7ad3ff";
    public string ColorNegativeItem { get; set; } = "#ffff9c7a";

    public int DataGridFontSize { get; set; } = 12;
    public int ChequeListPageSize { get; set; } = 50;
    public OrderType ChequeListQueryOrderType { get; set; } = OrderType.DESC;
    public int ChequeNotifyDays { get; set; } = 2;
    public bool ChequeNotify { get; set; } = true;
    public int InvoicePageSize { get; set; } = 50;
    public OrderType InvoiceListQueryOrderType { get; set; } = OrderType.DESC;
    public OrderType InvoiceDetailQueryOrderType { get; set; } = OrderType.DESC;
    public int TransactionListPageSize { get; set; } = 50;
    public int TransactionDetailPageSize { get; set; } = 50;
    public OrderType TransactionListQueryOrderType { get; set; } = OrderType.DESC;
    public OrderType TransactionDetailQueryOrderType { get; set; } = OrderType.DESC;
    public bool AutoSelectPersianLanguage { get; set; }
    public int TransactionShortcut1Id { get; set; } = 0;
    public int TransactionShortcut2Id { get; set; } = 0;
    public int TransactionShortcut3Id { get; set; } = 0;
    public string TransactionShortcut1Name { get; set; } = "میانبر یک";
    public string TransactionShortcut2Name { get; set; } = "میانبر دو";
    public string TransactionShortcut3Name { get; set; } = "میانبر سه";
    public bool AskToAddNotExistingProduct { get; set; } = true;
    public bool SearchWhenTyping { get; set; }
    public int CustomerListPageSize { get; set; } = 100;
    public OrderType CustomerListQueryOrderType { get; set; } = OrderType.DESC;
    public int ProductListPageSize { get; set; } = 100;
    public OrderType ProductListQueryOrderType { get; set; } = OrderType.DESC;
}