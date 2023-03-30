using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintInterface
{
    public class TransactionItemForPrintModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public long Amount { get; set; }
        public string CountString { get; set; }
        public decimal TotalPrice { get; set; }
        public string DateCreated { get; set; }
        public string TimeCreated { get; set; }
        public string DateUpdated { get; set; }
        public string TimeUpdated { get; set; }
        public string Descriptions { get; set; }
    }
}