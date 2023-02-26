using SharedLibrary.Validators;
using System.ComponentModel.DataAnnotations;

namespace SharedLibrary.DtoModels;

public class TransactionItemModel_DTO_Create_Update
{
    [Required(ErrorMessage = "Field is required")]
    public int TransactionId { get; set; }

    [Required(ErrorMessage = "Field is required")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Field is required")]
    public long Amount { get; set; }

    [Display(Name = "Count")]
    [Required(ErrorMessage = "Field is required")]
    [StringLength(50, ErrorMessage = "Maximum length is 50")]
    [CountStringIsValid]
    public string CountString { get; set; } = "0";

    [StringLength(50, ErrorMessage = "Maximum length is 50")]
    public string Descriptions { get; set; }
}