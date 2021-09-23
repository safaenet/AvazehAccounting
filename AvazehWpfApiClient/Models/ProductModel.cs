using System;
using System.Collections.Generic;
using System.Text;
using DotNetStandardCalculator;

namespace AvazehWpfApiClient.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public long BuyPrice { get; set; }
        public long SellPrice { get; set; }
        public string  Barcode { get; set; }
        public string CountString { get; set; }
        public string DateCreated { get; set; }
        public string TimeCreated { get; set; }
        public string DateUpdated { get; set; }
        public string TimeUpdated { get; set; }
        public string  Descriptions { get; set; }
        public double CountValue
        {
            get
            {
                double result;
                try
                {
                    result = StandardCalculator.CalculateFromString(CountString);
                }
                catch
                {
                    result = 0;
                }
                return result;
            }
        }
        public bool IsCountStringValid
        {
            get
            {
                try
                {
                    _ = StandardCalculator.CalculateFromString(CountString);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}