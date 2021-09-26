using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibraryCore.Models.Validators
{
    public class InvoiceValidator : AbstractValidator<InvoiceModel>
    {
        public InvoiceValidator()
        {
            RuleFor(i => i.Customer.Id)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .NotEqual(0).WithMessage("{PropertyName} of Phone Number cannot be Zero");

            RuleFor(i => i.LifeStatus).NotEmpty().WithMessage("{PropertyName} cannot be empty");

            RuleFor(i => i.DiscountValue).GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must be >= Zero");
        }
    }
}