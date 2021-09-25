using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvazehApiClient.Models
{
    public class InvoiceModel_DTO_Read
    {
        public InvoiceModel Invoice { get; set; }
        public double CustomerTotalBalance { get; set; }
    }
}
