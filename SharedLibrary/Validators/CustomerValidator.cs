﻿using FluentValidation;
using SharedLibrary.DalModels;

namespace SharedLibrary.Validators;

public class CustomerValidator : AbstractValidator<CustomerModel>
{
    public CustomerValidator()
    {
        RuleFor(c => c.FirstName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("{PropertyName} cannot be empty")
            .MaximumLength(50).WithMessage("{PropertyName} cannot be more than 50 characters ({TotalLength})");

        RuleFor(c => c.LastName)
            .MaximumLength(50).WithMessage("{PropertyName} cannot be more than 50 characters ({TotalLength})");

        RuleFor(c => c.CompanyName)
            .MaximumLength(50).WithMessage("{PropertyName} cannot be more than 50 characters ({TotalLength})");

        RuleForEach(c => c.PhoneNumbers).Cascade(CascadeMode.Stop).ChildRules(p =>
        {
            p.RuleFor(x => x.PhoneNumber).MaximumLength(15).WithMessage("{PropertyName} cannot be more than 15 characters ({TotalLength})");
                //p.RuleFor(x => x.PhoneNumber).Matches(@"/(0[0-9]{10}$)|(\+[0-9]{12}$)|(00[0-9]{1,3}[0-9]{10}$)/gm").WithMessage("{PropertyName} is not valid");
                //p.RuleFor(x => x.CustomerId).NotEqual(0).WithMessage("{PropertyName} of Phone Number cannot be Zero");
            });
    }
}