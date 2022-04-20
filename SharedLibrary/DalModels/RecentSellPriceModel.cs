using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.DalModels
{
    public class RecentSellPriceModel
    {
        public long SellPrice { get; set; }
        public string DateSold { get; set; }
        public string RecordName => SellPrice + " : " + DateSold;
    }
}