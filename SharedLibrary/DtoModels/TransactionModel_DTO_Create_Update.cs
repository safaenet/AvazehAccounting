using System;
using System.ComponentModel.DataAnnotations;

namespace SharedLibrary.DtoModels;

public class TransactionModel_DTO_Create_Update
{
    [Required]
    [StringLength(100, ErrorMessage = "Maximum length is 100")]
    public string FileName { get; set; }

    public string DateCreated { get; set; }

    [Display(Name = "Description")]
    public string Descriptions { get; set; }
}