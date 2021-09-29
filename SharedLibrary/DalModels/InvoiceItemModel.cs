using DotNetStandardCalculator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.DalModels
{
    public class InvoiceItemModel
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public ProductModel Product { get; set; }
        public long BuyPrice { get; set; }
        public long SellPrice { get; set; }
        public string CountString { get; set; } = "0";
        public string DateCreated { get; set; }
        public string TimeCreated { get; set; }
        public string DateUpdated { get; set; }
        public string TimeUpdated { get; set; }
        public bool Delivered { get; set; }
        public string Descriptions { get; set; }
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
                }
                catch
                {
                    return false;
                }
                return true;
            }
        }
        public double TotalBuyValue => CountValue * BuyPrice;
        public double TotalSellValue => CountValue * SellPrice;
        public double NetProfit => TotalSellValue - TotalBuyValue;
    }
}