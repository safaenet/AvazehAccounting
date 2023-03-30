using DotNetStandardCalculator;

namespace SharedLibrary.DalModels;

public class InvoiceItemModel
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public ProductModel Product { get; set; }
    public long BuyPrice { get; set; }
    public long SellPrice { get; set; }
    public string BarCode { get; set; }
    public string CountString { get; set; } = "1";
    public ProductUnitModel Unit { get; set; }
    public string DateCreated { get; set; }
    public string TimeCreated { get; set; }
    public string DateUpdated { get; set; }
    public string TimeUpdated { get; set; }
    public bool Delivered { get; set; }
    public string Descriptions { get; set; }
    public string DateTimeCreated => TimeCreated + " " + DateCreated;
    public string DateTimeUpdated => TimeUpdated + " " + DateUpdated;
    public decimal CountValue => StandardCalculator.CalculateFromString(CountString);
    public bool IsCountStringValid => StandardCalculator.IsCalculatable(CountString);
    public string CountPlusUnit
    {
        get
        {
            string result = CountValue.ToString();
            if (Unit != null && Unit.Id != 0) result += " " + Unit.UnitName;
            else result += " واحد";
            return result;
        }
    }
    public decimal TotalBuyValue => CountValue * BuyPrice;
    public decimal TotalSellValue => CountValue * SellPrice;
    public decimal NetProfit => TotalSellValue - TotalBuyValue;
}