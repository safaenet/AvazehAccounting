using DotNetStandardCalculator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.DalModels
{
    public class TransactionItemModel
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public string Title { get; set; }
        public long Amount { get; set; }
        public string CountString { get; set; } = "0";
        public string DateCreated { get; set; }
        public string TimeCreated { get; set; }
        public string DateUpdated { get; set; }
        public string TimeUpdated { get; set; }
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
        public double TotalValue => Amount * CountValue;
    }
}