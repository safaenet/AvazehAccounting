using DotNetStandardCalculator;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvazehApiClient.Models
{
    public class InvoiceItemModel_DTO_Create_Update
    {
        [Required(ErrorMessage = "Field is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Field must be greater than Zero")]
        public int InvoiceId { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Field must be greater than Zero")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [Range(0, long.MaxValue, ErrorMessage = "Field must be >= Zero")]
        public long BuyPrice { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [Range(0, long.MaxValue, ErrorMessage = "Field must be >= Zero")]
        public long SellPrice { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [StringLength(50, ErrorMessage = "Maximum length is 50")]
        [CountStringIsValid]
        public string CountString { get; set; }

        [Required(ErrorMessage = "Field is required")]
        public bool Delivered { get; set; }

        [StringLength(50, ErrorMessage = "Maximum length is 50")]
        public string Descriptions { get; set; }
    }
}