using SharedLibrary.Enums;
using System;

namespace SharedLibrary.DalModels;

/// <summary>
/// This model is for viewing Transactions in ListView
/// </summary>
public class TransactionListModel
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public string DateCreated { get; set; }
    public string DateUpdated { get; set; }
    public string Descriptions { get; set; }
    public decimal TotalPositiveItemsSum { get; set; }
    public decimal TotalNegativeItemsSum { get; set; }
    public decimal TotalBalance => TotalPositiveItemsSum + TotalNegativeItemsSum;
    public TransactionFinancialStatus TransactionFinancialStatus => TotalBalance == 0 ? TransactionFinancialStatus.Balanced : TotalBalance > 0 ? TransactionFinancialStatus.Positive : TransactionFinancialStatus.Negative;
}