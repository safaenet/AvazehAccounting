using SharedLibrary.DalModels;
using SharedLibrary.Enums;

namespace AvazehApiClient.DataAccess.Interfaces;

public interface ITransactionDetailManager : ICollectionManager<TransactionItemModel>
{
    TransactionFinancialStatus? FinStatus { get; set; }
    int TransactionId { get; set; }
}