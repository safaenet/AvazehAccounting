using System.Collections.Generic;

namespace SharedLibrary.Models;

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
    public List<PhoneNumberModel> PhoneNumbers { get; set; }
    public string FullName => $"{FirstName} {LastName}";
}