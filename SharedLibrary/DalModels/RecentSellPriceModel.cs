using System;

namespace SharedLibrary.DalModels;

public class RecentSellPriceModel
{
    public long SellPrice { get; set; }
    public DateTime DateSold { get; set; }
    public string RecordName => SellPrice + " : " + DateSold;
}