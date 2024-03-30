using DotNetStandardCalculator;

namespace SharedLibrary.DalModels;

public class InvoiceItemModel
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public ProductModel Product { get; set; }
    public long BuyPrice { get; set; }
    public long SellPrice { get; set; }
    public string CountString { get; set; }
    public string DateCreated { get; set; }
    public string DateUpdated { get; set; }
    public bool Delivered { get; set; }
    public string Descriptions { get; set; }
    public decimal CountValue => decimal.Round(StandardCalculator.CalculateFromString(CountString), 3);
    public bool IsCountStringValid => StandardCalculator.IsCalculatable(CountString);
    public decimal TotalBuyValue => CountValue * BuyPrice;
    public decimal TotalSellValue => CountValue * SellPrice;
    public decimal NetProfit => TotalSellValue - TotalBuyValue;
}