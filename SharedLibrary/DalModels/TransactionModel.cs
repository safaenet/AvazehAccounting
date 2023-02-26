using SharedLibrary.Enums;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace SharedLibrary.DalModels;

public class TransactionModel
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime? DateUpdated { get; set; }
    public ObservableCollection<TransactionItemModel> Items { get; set; }
    public string Descriptions { get; set; }

    public double PositiveItemsSum => Items == null ? 0 : Items.Where(i => i.TotalValue > 0).Sum(i => i.TotalValue);
    public double NegativeItemsSum => Items == null ? 0 : Items.Where(i => i.TotalValue < 0).Sum(i => i.TotalValue);
    public double Balance => PositiveItemsSum + NegativeItemsSum;
    public double TotalPositiveItemsSum { get; set; }
    public double TotalNegativeItemsSum { get; set; }
    public double TotalBalance => TotalPositiveItemsSum + TotalNegativeItemsSum;
    public TransactionFinancialStatus TransactionFinancialStatus => Balance == 0 ? TransactionFinancialStatus.Balanced : Balance > 0 ? TransactionFinancialStatus.Positive : TransactionFinancialStatus.Negative;
}