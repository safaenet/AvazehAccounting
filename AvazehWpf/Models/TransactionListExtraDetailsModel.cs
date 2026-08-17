using SharedLibrary.Contracts;
using SharedLibrary.Enums;

namespace AvazehWpf.Models
{
    public class TransactionListExtraDetailsModel : TransactionSummaryDTO
    {
        public string DateTimeCreated => TimeCreated + " " + DateCreated;
        public string DateTimeUpdated => TimeUpdated + " " + DateUpdated;
        public decimal TotalBalance => TotalPositiveItemsSum + TotalNegativeItemsSum;
        public TransactionFinancialStatus TransactionFinancialStatus => TotalBalance == 0 ? TransactionFinancialStatus.Balanced : TotalBalance > 0 ? TransactionFinancialStatus.Positive : TransactionFinancialStatus.Negative;
    }
}