using SharedLibrary.DalModels;
using SharedLibrary.Validators;
using System;
using System.ComponentModel.DataAnnotations;

namespace SharedLibrary.DtoModels //DTO Models
;

public class InvoiceItemModel_DTO_Create_Update
{
    public int Id { get; set; }

    [Display(Name = "Invoice ID")]
    [Required(ErrorMessage = "Field is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Field must be greater than Zero")]
    public int InvoiceId { get; set; }

    [Display(Name = "Product ID")]
    [Required(ErrorMessage = "Field is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Field must be greater than Zero")]
    public int ProductId { get; set; }

    [Display(Name = "Buy Price")]
    [Required(ErrorMessage = "Field is required")]
    [Range(0, long.MaxValue, ErrorMessage = "Field must be >= Zero")]
    public long BuyPrice { get; set; }

    [Display(Name = "Sell Price")]
    [Required(ErrorMessage = "Field is required")]
    [Range(0, long.MaxValue, ErrorMessage = "Field must be >= Zero")]
    public long SellPrice { get; set; }

    [Display(Name = "Count")]
    [Required(ErrorMessage = "Field is required")]
    [StringLength(50, ErrorMessage = "Maximum length is 50")]
    [CountStringIsValid]
    public string CountString { get; set; }

    public string CountValue { get; set; }

    public string DateCreated { get; set; }

    public string DateUpdated { get; set; }

    [Display(Name = "Delivered")]
    [Required(ErrorMessage = "Field is required")]
    public bool Delivered { get; set; }

    [Display(Name = "Description")]
    [StringLength(50, ErrorMessage = "Maximum length is 50")]
    public string Descriptions { get; set; }
}