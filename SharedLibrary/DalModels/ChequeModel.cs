using SharedLibrary.Enums;
using System;
using System.Collections.ObjectModel;

namespace SharedLibrary.DalModels;

public class ChequeModel
{
    public int Id { get; set; }
    public string Drawer { get; set; }
    public string Orderer { get; set; }
    public long PayAmount { get; set; }
    public string About { get; set; }
    public string IssueDate { get; set; }
    public string DueDate { get; set; }
    public string DateCreated { get; set; }
    public string BankName { get; set; }
    public string SerialNumber { get; set; }
    public string Identifier { get; set; } //Sayyaad Code
    public string Descriptions { get; set; }
    public string PayAmountInPersian { get; }
    public ChequeStatusTypes StatusType { get; set; }
    public string StatusDate { get; set; }
    public string StatusText { get; set; }
}