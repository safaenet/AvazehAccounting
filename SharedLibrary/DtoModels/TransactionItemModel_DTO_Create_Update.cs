﻿using SharedLibrary.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.DtoModels
{
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

        [Required]
        public string DateCreated { get; set; }

        [Required]
        public string TimeCreated { get; set; }

        [StringLength(50, ErrorMessage = "Maximum length is 50")]
        public string Descriptions { get; set; }
    }
}