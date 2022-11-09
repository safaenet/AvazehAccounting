using FluentValidation;
using SharedLibrary.DalModels;

namespace SharedLibrary.Validators
{
    public class ProductValidator : AbstractValidator<ProductModel>
    {
        public ProductValidator()
        {
            RuleFor(p => p.ProductName).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .MaximumLength(100).WithMessage("{PropertyName} cannot be more than 100 characters ({TotalLength})");
            RuleFor(p => p.BuyPrice).GreaterThan(-1).WithMessage("{PropertyName} must be >= 0");
            RuleFor(p => p.SellPrice).GreaterThan(-1).WithMessage("{PropertyName} must be >= 0");
            RuleFor(p => p.Barcode).MaximumLength(15).WithMessage("{PropertyName} cannot be more than 15 characters ({TotalLength})");
            RuleFor(p => p.IsCountStringValid).Equal(true).WithMessage("Count is not valid.");
        }
    }
}