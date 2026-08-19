using SharedLibrary.Enums;
using System.Collections.Generic;
using System.Linq;

namespace SharedLibrary.Models;

public class TransactionModel
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public string DateCreated { get; set; }
    public string TimeCreated { get; set; }
    public string DateUpdated { get; set; }
    public string TimeUpdated { get; set; }
    public List<TransactionItemModel> Items { get; set; }
    public string Descriptions { get; set; }

    public decimal PositiveItemsSum => Items == null ? 0 : Items.Where(i => i.TotalValue > 0).Sum(i => i.TotalValue); //Total amount of positive items in the current loaded items - loaded by pagination.
    public decimal NegativeItemsSum => Items == null ? 0 : Items.Where(i => i.TotalValue < 0).Sum(i => i.TotalValue); //Total amount of negative items in the current loaded items - loaded by pagination.
    public decimal Balance => PositiveItemsSum + NegativeItemsSum;
    public decimal TotalPositiveItemsSum { get; set; } //Total amount of positive items in the whole transaction.
    public decimal TotalNegativeItemsSum { get; set; } //Total amount of negative items in the whole transaction.
    public decimal TotalBalance => TotalPositiveItemsSum + TotalNegativeItemsSum;
    public TransactionFinancialStatus TransactionFinancialStatus => Balance == 0 ? TransactionFinancialStatus.Balanced : Balance > 0 ? TransactionFinancialStatus.Positive : TransactionFinancialStatus.Negative;
}