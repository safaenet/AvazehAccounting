using System;

namespace SharedLibrary.DalModels;

public class InvoicePaymentModel
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public string DateCreated { get; set; }
    public string TimeCreated { get; set; }
    public string DateUpdated { get; set; }
    public string TimeUpdated { get; set; }
    public double PayAmount { get; set; }
    public string Descriptions { get; set; }
}