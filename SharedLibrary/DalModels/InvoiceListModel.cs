using SharedLibrary.Enums;
using System;

namespace SharedLibrary.DalModels;

/// <summary>
/// This model is for viewing Invoices in ListView
/// </summary>
public class InvoiceListModel
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerFullName { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }
    public double TotalInvoiceSum { get; set; }
    public double TotalPayments { get; set; }
    public InvoiceLifeStatus LifeStatus { get; set; }
    public double TotalBalance => TotalInvoiceSum - TotalPayments;
    public InvoiceFinancialStatus InvoiceFinancialStatus => TotalBalance == 0 ? InvoiceFinancialStatus.Balanced : TotalBalance > 0 ? InvoiceFinancialStatus.Deptor : InvoiceFinancialStatus.Creditor;
}