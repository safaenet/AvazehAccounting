using System;

namespace SharedLibrary.DalModels;

public class TransactionItemForPrintModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public long Amount { get; set; }
    public string CountString { get; set; }
    public double TotalPrice { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }
    public string Descriptions { get; set; }
}