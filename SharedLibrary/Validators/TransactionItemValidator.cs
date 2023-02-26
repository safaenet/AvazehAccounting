using FluentValidation;
using SharedLibrary.DalModels;

namespace SharedLibrary.Validators;

public class TransactionItemValidator : AbstractValidator<TransactionItemModel>
{
    public TransactionItemValidator()
    {
        RuleFor(x => x.TransactionId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("{PropertyName} cannot be empty")
            .NotEqual(0).WithMessage("{PropertyName} cannot be Zero");

        RuleFor(x => x.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("{PropertyName} cannot be empty")
            .MaximumLength(100).WithMessage("{PropertyName} cannot be more than 100 characters ({TotalLength})");

        RuleFor(x => x.Amount)
            .NotEmpty().WithMessage("{PropertyName} cannot be empty");

        RuleFor(x => x.CountString).MaximumLength(50).WithMessage("{PropertyName} cannot be more than 50 characters ({TotalLength})");

        RuleFor(x => x.IsCountStringValid).Equal(true).WithMessage("Count is not valid");

        RuleFor(x => x.Descriptions).MaximumLength(50).WithMessage("{PropertyName} cannot be more than 50 characters ({TotalLength})");
    }
}