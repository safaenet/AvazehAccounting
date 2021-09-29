using FluentValidation;
using SharedLibrary.DalModels;

namespace SharedLibrary.Validators
{
    public class InvoiceItemValidator : AbstractValidator<InvoiceItemModel>
    {
        public InvoiceItemValidator()
        {
            RuleFor(x => x.InvoiceId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .NotEqual(0).WithMessage("{PropertyName} cannot be Zero");

            RuleFor(x => x.Product.Id)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .NotEqual(0).WithMessage("{PropertyName} cannot be Zero");

            RuleFor(x => x.BuyPrice).GreaterThan(-1).WithMessage("{PropertyName} must be >= 0");

            RuleFor(x => x.SellPrice).GreaterThan(-1).WithMessage("{PropertyName} must be >= 0");

            RuleFor(x => x.CountString)
                .MaximumLength(50).WithMessage("{PropertyName} cannot be more than 50 characters ({TotalLength})");
            RuleFor(x => x.IsCountStringValid).Equal(true).WithMessage("Count is not valid");
            RuleFor(x => x.Delivered).NotEmpty().WithMessage("{PropertyName} cannot be empty");
            RuleFor(x => x.Descriptions).MaximumLength(50).WithMessage("{PropertyName} cannot be more than 50 characters ({TotalLength})");
        }
    }
}