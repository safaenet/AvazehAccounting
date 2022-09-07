using SharedLibrary.DalModels;
using SharedLibrary.SettingsModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.DalModels
{
    public class PrintTransactionModel
    {
        public int TransactionId;
        public List<TransactionItemForPrintModel> Items;
        public string FileName;
        public string TransactionDateCreated;
        public string TransactionDescription = "";
        public string TransactionFinStatus;
        public double TotalBalance;
        public double TotalPositiveItemsSum;
        public double TotalNegativeItemsSum;

        public bool PrintTransactionDescription;
        public bool PrintUserDescription;
        public bool PrintTransactionId = true;
        public bool PrintDate = true;
        public PrintSettingsModel PrintSettings = new();
    }
}