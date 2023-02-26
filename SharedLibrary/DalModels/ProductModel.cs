using System;
using DotNetStandardCalculator;

namespace SharedLibrary.DalModels;

public class ProductModel
{
    public int Id { get; set; }
    public string ProductName { get; set; }
    public long BuyPrice { get; set; } = 0;
    public long SellPrice { get; set; } = 0;
    public string Barcode { get; set; }
    public string CountString { get; set; } = "0";
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }
    public string Descriptions { get; set; }
    public bool IsActive { get; set; } = true;
    public double CountValue => StandardCalculator.CalculateFromString(CountString);
    public bool IsCountStringValid => StandardCalculator.IsCalculatable(CountString);
}