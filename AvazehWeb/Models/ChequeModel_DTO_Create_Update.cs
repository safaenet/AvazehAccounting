using DataLibraryCore.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace AvazehWeb.Models
{
    public class ChequeModel_DTO_Create_Update
    {
        public string Drawer { get; set; }
        public string Orderer { get; set; }
        public long PayAmount { get; set; }
        public string About { get; set; }
        public string IssueDate { get; set; }
        public string DueDate { get; set; }
        public string BankName { get; set; }
        public string Serial { get; set; }
        public string Identifier { get; set; } //Sayyaad Code
        public string Descriptions { get; set; }
        public ObservableCollection<ChequeEventModel> Events { get; set; }
    }
}