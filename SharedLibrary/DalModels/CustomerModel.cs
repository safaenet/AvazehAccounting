using System;
using System.Collections.ObjectModel;

namespace SharedLibrary.DalModels;

public class CustomerModel
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string CompanyName { get; set; }
    public string PhoneNumber { get; set; }
    public string EmailAddress { get; set; }
    public string PostAddress { get; set; }
    public string DateJoined { get; set; }
    public string Descriptions { get; set; }
}