using System;
using System.Collections.Generic;
using System.Text;
using DotNetStandardCalculator;

namespace SharedLibrary.DalModels
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public long BuyPrice { get; set; } = 0;
        public long SellPrice { get; set; } = 0;
        public string  Barcode { get; set; }
        public string CountString { get; set; } = "0";
        public string DateCreated { get; set; }
        public string TimeCreated { get; set; }
        public string DateUpdated { get; set; }
        public string TimeUpdated { get; set; }
        public string  Descriptions { get; set; }
        public double CountValue => StandardCalculator.CalculateFromString(CountString);
        public bool IsCountStringValid => StandardCalculator.IsCalculatable(CountString);
    }
}