using System;

namespace SharedLibrary.DalModels
{
    public class InvoicePaymentModel
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public double PayAmount { get; set; }
        public string Descriptions { get; set; }
    }
}