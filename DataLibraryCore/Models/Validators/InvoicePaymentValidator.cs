using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibraryCore.Models.Validators
{
    public class InvoicePaymentValidator : AbstractValidator<InvoicePaymentModel>
    {
        public InvoicePaymentValidator()
        {
            RuleFor(x => x.InvoiceId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .NotEqual(0).WithMessage("{PropertyName} cannot be Zero");

            RuleFor(x => x.PayAmount).GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must be >= Zero");
        }
    }
}