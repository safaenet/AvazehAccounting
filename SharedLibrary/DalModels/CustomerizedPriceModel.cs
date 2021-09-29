using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.DalModels
{
    public class CustomerizedPriceModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public long SellPrice { get; set; }
        public string  DateAdded { get; set; }
    }
}
