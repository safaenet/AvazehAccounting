using SharedLibrary.SecurityAndSettingsModels;
using System.Collections.Generic;

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

        public int TransactionType;
        public bool PrintTransactionDescription;
        public bool PrintUserDescription;
        public bool PrintTransactionId = true;
        public bool PrintDate = true;
        public PrintSettingsModel PrintSettings = new();
    }
}