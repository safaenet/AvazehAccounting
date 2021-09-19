using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvazehWebAPI.Models //DTO Models
{
    public class ProductModel_DTO_Create_Update
    {
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

        [Display(Name = "Description")]
        public string Descriptions { get; set; }
    }
}