using System;

namespace SharedLibrary.Models;

public class RecentSellPriceModel
{
    public long SellPrice { get; set; }
    public string DateSold { get; set; }
    public string RecordName => SellPrice + " : " + DateSold;
}