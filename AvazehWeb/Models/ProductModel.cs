using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvazehWeb.Models
{
    public class ProductModel
    {
        public int Id { get; set; }

        [Display(Name ="Product Name", Prompt ="Product name. Max length is 100 characters")]
        [Required(ErrorMessage = "Field is required")]
        [StringLength(100, ErrorMessage = "Maximum length is 100")]
        public string ProductName { get; set; }

        [Display(Name ="Buy price", Prompt ="Buy Price")]
        [Required(ErrorMessage = "Field is required")]
        public long BuyPrice { get; set; }

        [Display(Name = "Sell price", Prompt = "Sell Price")]
        [Required(ErrorMessage = "Field is required")]
        public long SellPrice { get; set; }

        [Display(Name ="Barcode", Prompt ="Barcode. Max length is 15 characters")]
        [StringLength(15, ErrorMessage = "Maximum length is 15")]
        public string Barcode { get; set; }

        [Display(Name ="Count", Prompt ="Count. Max length is 50 characters")]
        [StringLength(50, ErrorMessage = "Maximum length is 50")]
        public string CountString { get; set; }

        [Display(Name ="Created Date")]
        [RegularExpression(@"([12]\d{3}\/(0[1-9]|1[0-2])\/(0[1-9]|[12]\d|3[01]))$", ErrorMessage = "Enter a valid date")]
        public string DateCreated { get; set; }

        [Display(Name = "Created Time")]
        [RegularExpression(@"^(?:(?:([01]?\d|2[0-3]):)?([0-5]?\d):)?([0-5]?\d)$", ErrorMessage = "Enter a valid time")]
        public string TimeCreated { get; set; }

        [Display(Name = "Updated Date")]
        [RegularExpression(@"([12]\d{3}\/(0[1-9]|1[0-2])\/(0[1-9]|[12]\d|3[01]))$", ErrorMessage = "Enter a valid date")]
        public string DateUpdated { get; set; }

        [Display(Name = "Updated Time")]
        [RegularExpression(@"^(?:(?:([01]?\d|2[0-3]):)?([0-5]?\d):)?([0-5]?\d)$", ErrorMessage = "Enter a valid time")]
        public string TimeUpdated { get; set; }

        [Display(Name = "Description")]
        public string Descriptions { get; set; }
    }
}