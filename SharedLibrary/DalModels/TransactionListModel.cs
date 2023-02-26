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
    public DateTime DateCreated { get; set; }
    public DateTime? DateUpdated { get; set; }
    public string Descriptions { get; set; }
    public double TotalPositiveItemsSum { get; set; }
    public double TotalNegativeItemsSum { get; set; }
    public double TotalBalance => TotalPositiveItemsSum + TotalNegativeItemsSum;
    public TransactionFinancialStatus TransactionFinancialStatus => TotalBalance == 0 ? TransactionFinancialStatus.Balanced : TotalBalance > 0 ? TransactionFinancialStatus.Positive : TransactionFinancialStatus.Negative;
}