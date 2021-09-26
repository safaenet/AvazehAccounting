using DataLibraryCore.Models;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace AvazehWeb.Models
{
    public class InvoiceModel_DTO_Create_Update
    {
        public int CustomerId { get; set; }

        [Display(Name = "Discount Type")]
        public DiscountTypes DiscountType { get; set; }

        [Display(Name = "Discount Value")]
        [Range(0, double.MaxValue, ErrorMessage = "Field must be greater than Zero")]
        public double DiscountValue { get; set; }

        [Display(Name = "Description")]
        public string Descriptions { get; set; }
        public InvoiceLifeStatus LifeStatus { get; set; }
    }
}