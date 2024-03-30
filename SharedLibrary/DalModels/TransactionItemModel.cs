using DotNetStandardCalculator;
using System;

namespace SharedLibrary.DalModels;

public class TransactionItemModel
{
    public int Id { get; set; }
    public int TransactionId { get; set; }
    public string Title { get; set; }
    public long Amount { get; set; }
    public string CountString { get; set; } = "1";
    public string DateCreated { get; set; }
    public string DateUpdated { get; set; }
    public string Descriptions { get; set; }
    public decimal CountValue => StandardCalculator.CalculateFromString(CountString);
    public bool IsCountStringValid => StandardCalculator.IsCalculatable(CountString);
    public decimal TotalValue => Amount * CountValue;
}