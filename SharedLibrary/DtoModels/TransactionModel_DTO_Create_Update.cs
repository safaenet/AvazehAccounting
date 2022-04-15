using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.DtoModels
{
    public class TransactionModel_DTO_Create_Update
    {
        [Required]
        [StringLength(100, ErrorMessage = "Maximum length is 100")]
        public string FileName { get; set; }

        [Required]
        public string DateCreated { get; set; }

        [Required]
        public string TimeCreated { get; set; }

        [Display(Name = "Description")]
        public string Descriptions { get; set; }
    }
}