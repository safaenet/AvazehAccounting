using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvazehWeb.Models //DTO Models
{
    public class InvoicePaymentModel_DTO_Create_Update
    {
        public int InvoiceId { get; set; }

        [Display(Name = "Payment Amount")]
        [Required(ErrorMessage = "Field is required")]
        [Range(1, double.MaxValue, ErrorMessage = "Field must be greater than Zero")]
        public double PayAmount { get; set; }

        [Display(Name = "Description")]
        public string Descriptions { get; set; }
    }
}