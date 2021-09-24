using DotNetStandardCalculator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvazehWpfApiClient.Models
{
    public class InvoiceItemModel_DTO_Create_Update
    {
        public int InvoiceId { get; set; }
        public int ProductId { get; set; }
        public long BuyPrice { get; set; }
        public long SellPrice { get; set; }
        public string CountString { get; set; }
        public bool Delivered { get; set; }
        public string Descriptions { get; set; }
    }
}