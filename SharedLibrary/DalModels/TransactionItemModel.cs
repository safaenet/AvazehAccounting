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
    public DateTime DateCreated { get; set; }
    public DateTime? DateUpdated { get; set; }
    public string Descriptions { get; set; }
    public double CountValue => StandardCalculator.CalculateFromString(CountString);
    public bool IsCountStringValid => StandardCalculator.IsCalculatable(CountString);
    public double TotalValue => Amount * CountValue;
}