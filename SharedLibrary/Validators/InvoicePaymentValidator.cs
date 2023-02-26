using FluentValidation;
using SharedLibrary.DalModels;

namespace SharedLibrary.Validators;

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