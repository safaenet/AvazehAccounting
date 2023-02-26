using DotNetStandardCalculator;
using System;

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
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }
    public bool Delivered { get; set; }
    public string Descriptions { get; set; }
    public double CountValue => StandardCalculator.CalculateFromString(CountString);
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
    public double TotalBuyValue => CountValue * BuyPrice;
    public double TotalSellValue => CountValue * SellPrice;
    public double NetProfit => TotalSellValue - TotalBuyValue;
}