using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.DalModels
{
    /// <summary>
    /// This model is for viewing Transactions in ListView
    /// </summary>
    public class TransactionListModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string DateCreated { get; set; }
        public string TimeCreated { get; set; }
        public string DateUpdated { get; set; }
        public string TimeUpdated { get; set; }
        public string Descriptions { get; set; }
        public double TotalPositiveItemsSum { get; set; }
        public double TotalNegativeItemsSum { get; set; }
        public double TotalBalance => TotalPositiveItemsSum + TotalNegativeItemsSum;
        public TransactionFinancialStatus TransactionFinancialStatus => TotalBalance == 0 ? TransactionFinancialStatus.Balanced : TotalBalance > 0 ? TransactionFinancialStatus.Positive : TransactionFinancialStatus.Negative;
    }
}