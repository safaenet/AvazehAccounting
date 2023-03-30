using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintInterface
{
    public class PrintTransactionModel
    {
        public int TransactionId;
        public List<TransactionItemForPrintModel> Items;
        public string FileName;
        public string TransactionDateCreated;
        public string TransactionDescription = "";
        public string TransactionFinStatus;
        public decimal TotalBalance;
        public decimal TotalPositiveItemsSum;
        public decimal TotalNegativeItemsSum;

        public int TransactionType;
        public bool PrintTransactionDescription;
        public bool PrintUserDescription;
        public bool PrintTransactionId = true;
        public bool PrintDate = true;
        public PrintSettingsModel PrintSettings;
    }
}