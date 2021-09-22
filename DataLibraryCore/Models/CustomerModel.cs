using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DataLibraryCore.Models
{
    public class CustomerModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string EmailAddress { get; set; }
        public string PostAddress { get; set; }
        public string DateJoined { get; set; }
        public string Descriptions { get; set; }
        public double TotalBalance { get; set; } //This should be deleted
        public ObservableCollection<PhoneNumberModel> PhoneNumbers { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }
}