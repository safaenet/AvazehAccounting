﻿using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.DalModels
{
    public class TransactionModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string DateCreated { get; set; }
        public string TimeCreated { get; set; }
        public string DateUpdated { get; set; }
        public string TimeUpdated { get; set; }
        public ObservableCollection<TransactionItemModel> Items { get; set; }
        public string Descriptions { get; set; }

        public ObservableCollection<TransactionItemModel> PositiveItems => Items.Where(i => i.TotalValue > 0) as ObservableCollection<TransactionItemModel>;
        public ObservableCollection<TransactionItemModel> NegativeItems => Items.Where(i => i.TotalValue < 0) as ObservableCollection<TransactionItemModel>;

        public double TotalPositiveItemsSum => PositiveItems == null || PositiveItems.Count == 0 ? 0 : PositiveItems.Sum(i => i.TotalValue);
        public double TotalNegativeItemsSum => NegativeItems == null || NegativeItems.Count == 0 ? 0 : NegativeItems.Sum(i => i.TotalValue);
        public double TotalBalance => TotalPositiveItemsSum + TotalNegativeItemsSum;
        public TransactionFinancialStatus TransactionFinancialStatus => TotalBalance == 0 ? TransactionFinancialStatus.Balanced : TotalBalance > 0 ? TransactionFinancialStatus.Positive : TransactionFinancialStatus.Negative;
    }
}