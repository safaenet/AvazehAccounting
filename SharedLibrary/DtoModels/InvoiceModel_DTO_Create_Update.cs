using SharedLibrary.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace SharedLibrary.DtoModels;

public class InvoiceModel_DTO_Create_Update
{
    public int CustomerId { get; set; }
    public string About { get; set; }
    public string DateCreated { get; set; }
    public string TimeCreated { get; set; }

    [Display(Name = "Discount Type")]
    public DiscountTypes DiscountType { get; set; }

    [Display(Name = "Discount Value")]
    [Range(0, double.MaxValue, ErrorMessage = "Field must be >= Zero")]
    public double DiscountValue { get; set; }

    [Display(Name = "Description")]
    public string Descriptions { get; set; }
    public InvoiceLifeStatus LifeStatus { get; set; }
}