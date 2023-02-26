using System;
using System.ComponentModel.DataAnnotations;

namespace SharedLibrary.DtoModels //DTO Models
;

public class InvoicePaymentModel_DTO_Create_Update
{
    public int InvoiceId { get; set; }

    [Display(Name = "Payment Amount")]
    [Required(ErrorMessage = "Field is required")]
    [Range(1, double.MaxValue, ErrorMessage = "Field must be greater than Zero")]
    public double PayAmount { get; set; }

    [Display(Name = "Description")]
    public string Descriptions { get; set; }
}