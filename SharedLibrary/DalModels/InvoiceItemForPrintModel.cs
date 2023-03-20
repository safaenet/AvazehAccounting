using System;

namespace SharedLibrary.DalModels;

public class InvoiceItemForPrintModel
{
    public int Id { get; set; }
    public string ProductName { get; set; }
    public double SellPrice { get; set; }
    public string CountString { get; set; }
    public double TotalPrice { get; set; }
    public string DateCreated { get; set; }
    public string TimeCreated { get; set; }
    public string DateUpdated { get; set; }
    public string TimeUpdated { get; set; }
    public bool Delivered { get; set; }
    public string Descriptions { get; set; }
    public string ProductUnitName { get; set; }
}