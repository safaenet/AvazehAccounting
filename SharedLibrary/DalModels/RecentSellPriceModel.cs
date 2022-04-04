using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.DalModels
{
    public class RecentSellPriceModel
    {
        //public int Id { get; set; }
        //public int CustomerId { get; set; }
        //public int ProductId { get; set; }
        public long SellPrice { get; set; }
        public string DateSold { get; set; }
        public string RecordName => SellPrice + " : " + DateSold;
    }
}