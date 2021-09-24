using System.Collections.ObjectModel;

namespace AvazehWpfApiClient.Models
{
    public class InvoiceModel_DTO_Create_Update
    {
        public int CustomerId { get; set; }
        public DiscountTypes DiscountType { get; set; }
        public double DiscountValue { get; set; }
        public string Descriptions { get; set; }
        public InvoiceLifeStatus LifeStatus { get; set; }
    }
}