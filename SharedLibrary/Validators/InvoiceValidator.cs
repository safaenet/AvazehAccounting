using FluentValidation;
using SharedLibrary.DalModels;

namespace SharedLibrary.Validators;

public class InvoiceValidator : AbstractValidator<InvoiceModel>
{
    public InvoiceValidator()
    {
        RuleFor(i => i.Customer.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("{PropertyName} cannot be empty")
            .NotEqual(0).WithMessage("{PropertyName} cannot be Zero");

        RuleFor(i => i.DiscountValue).GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must be >= Zero");
    }
}