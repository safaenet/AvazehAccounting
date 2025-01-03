﻿using SharedLibrary.DalModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace SharedLibrary.DtoModels //DTO Models
;

public class CustomerModel_DTO_Create_Update
{
    [Display(Name = "First Name", Prompt = "First Name. Max length is 50 characters")]
    [Required(ErrorMessage = "Field is required")]
    [StringLength(50, ErrorMessage = "Maximum length is 50")]
    public string FirstName { get; set; }

    [Display(Name = "Last Name", Prompt = "Last Name. Max length is 50 characters")]
    [StringLength(50, ErrorMessage = "Maximum length is 50")]
    public string LastName { get; set; }

    [Display(Name = "Company", Prompt = "Company name. Max length is 50 characters")]
    [StringLength(50, ErrorMessage = "Maximum length is 50")]
    public string CompanyName { get; set; }

    [Display(Name = "Email Address", Prompt = "Email Address. Max length is 100 characters")]
    [StringLength(100, ErrorMessage = "Maximum length is 100")]
    [EmailAddress(ErrorMessage = "Email is not valid")]
    public string EmailAddress { get; set; }

    [Display(Name = "Post Address", Prompt = "Post Address. Max length is 100 characters")]
    public string PostAddress { get; set; }
    public string DateJoined { get; set; }

    public ObservableCollection<PhoneNumberModel> PhoneNumbers { get; set; }

    [Display(Name = "Description")]
    public string Descriptions { get; set; }
}