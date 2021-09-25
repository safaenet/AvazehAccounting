using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvazehApiClient.Models.Validators
{
    public class InvoicePaymentValidator : AbstractValidator<InvoicePaymentModel>
    {
        public InvoicePaymentValidator()
        {
            RuleFor(x=>x.InvoiceId)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .NotEqual(0).WithMessage("{PropertyName} of Phone Number cannot be Zero");

            RuleFor(x => x.PayAmount)
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than Zero");
        }
    }
}