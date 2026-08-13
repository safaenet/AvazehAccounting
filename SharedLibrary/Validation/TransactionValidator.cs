using FluentValidation;
using SharedLibrary.Models;

namespace SharedLibrary.Validation;

public class TransactionValidator : AbstractValidator<TransactionModel>
{
    public TransactionValidator()
    {
        RuleFor(x => x.FileName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("{PropertyName} cannot be empty")
            .MaximumLength(100).WithMessage("{PropertyName} cannot be more than 100 characters ({TotalLength})");
    }
}