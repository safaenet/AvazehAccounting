using SharedLibrary.DalModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace SharedLibrary.DtoModels //DTO Models
;

public class ChequeModel_DTO_Create_Update
{
    [Required(ErrorMessage = "Field is required")]
    [StringLength(50, ErrorMessage = "Maximum length is 50")]
    public string Drawer { get; set; }

    [Required(ErrorMessage = "Field is required")]
    [StringLength(50, ErrorMessage = "Maximum length is 50")]
    public string Orderer { get; set; }

    [Required(ErrorMessage = "Field is required")]
    public long PayAmount { get; set; }

    [StringLength(100, ErrorMessage = "Maximum length is 100")]
    public string About { get; set; }

    [Required(ErrorMessage = "Field is required")]
    public DateOnly IssueDate { get; set; }

    [Required(ErrorMessage = "Field is required")]
    public DateOnly DueDate { get; set; }

    [Required(ErrorMessage = "Field is required")]
    [StringLength(50, ErrorMessage = "Maximum length is 50")]
    public string BankName { get; set; }

    [StringLength(25, ErrorMessage = "Maximum length is 25")]
    public string Serial { get; set; }

    [StringLength(20, ErrorMessage = "Maximum length is 20")]
    public string Identifier { get; set; } //Sayyaad Code
    public string Descriptions { get; set; }
    public ObservableCollection<ChequeEventModel> Events { get; set; }
}