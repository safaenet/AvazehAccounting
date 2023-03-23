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
    public string About { get; set; }
    public string DateCreated { get; set; }
    public string TimeCreated { get; set; }
    public string DateUpdated { get; set; }
    public string TimeUpdated { get; set; }
    public double TotalInvoiceSum { get; set; }
    public double TotalPayments { get; set; }
    public InvoiceLifeStatus LifeStatus { get; set; }
    public int? PrevInvoiceId { get; set; }
    public int PrevInvoiceBalance { get; set; }
    public int? FwdInvoiceId { get; set; }
    public double TotalInvoiceBalance => TotalInvoiceSum - TotalPayments;
    public double TotalBalance => TotalInvoiceBalance + PrevInvoiceBalance;
    public string InvoiceTitle => string.IsNullOrEmpty(About) ? CustomerFullName : CustomerFullName + " - " + About;
    public string DateTimeCreated => TimeCreated + " " + DateCreated;
    public string DateTimeUpdated => TimeUpdated + " " + DateUpdated;
    public InvoiceFinancialStatus InvoiceFinancialStatus => TotalBalance == 0 ? InvoiceFinancialStatus.Balanced : TotalBalance > 0 ? InvoiceFinancialStatus.Deptor : InvoiceFinancialStatus.Creditor;
    //public InvoiceFinancialStatus InvoiceFinancialStatus => (FwdInvoiceId is not null and > 0) ? InvoiceFinancialStatus.Outstanding : TotalBalance == 0 ? InvoiceFinancialStatus.Balanced : TotalBalance > 0 ? InvoiceFinancialStatus.Deptor : InvoiceFinancialStatus.Creditor;
}