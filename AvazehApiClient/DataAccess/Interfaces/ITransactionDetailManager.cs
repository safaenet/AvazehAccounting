using AvazehApiClient.DataAccess.Interfaces;
using FluentValidation.Results;
using SharedLibrary.DalModels;
using SharedLibrary.Enums;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace AvazehApiClient.DataAccess.Interfaces
{
    public interface ITransactionDetailManager : ICollectionManager<TransactionItemModel>
    {
        TransactionFinancialStatus? FinStatus { get; set; }
        int TransactionId { get; set; }
    }
}