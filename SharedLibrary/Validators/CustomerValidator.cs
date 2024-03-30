using FluentValidation;
using SharedLibrary.DalModels;

namespace SharedLibrary.Validators;

public class CustomerValidator : AbstractValidator<CustomerModel>
{
    public CustomerValidator()
    {
        RuleFor(c => c.FullName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("{PropertyName} cannot be empty")
            .MaximumLength(50).WithMessage("{PropertyName} cannot be more than 50 characters ({TotalLength})");

        RuleFor(c => c.CompanyName)
            .MaximumLength(50).WithMessage("{PropertyName} cannot be more than 50 characters ({TotalLength})");
    }
}