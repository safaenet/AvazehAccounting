using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvazehWpfApiClient.Models
{
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
        [RegularExpression(@"([12]\d{3}\/(0[1-9]|1[0-2])\/(0[1-9]|[12]\d|3[01]))$", ErrorMessage = "Issue Date is not valid (YYYY/mm/dd)")]
        public string IssueDate { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [RegularExpression(@"([12]\d{3}\/(0[1-9]|1[0-2])\/(0[1-9]|[12]\d|3[01]))$", ErrorMessage = "Due Date is not valid (YYYY/mm/dd)")]
        public string DueDate { get; set; }

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
}