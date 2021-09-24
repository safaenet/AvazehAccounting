using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibraryCore.Models.Validators
{
    internal class ProductValidator : AbstractValidator<ProductModel>
    {
        internal ProductValidator()
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