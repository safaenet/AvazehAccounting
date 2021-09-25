using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AvazehApiClient.Models.Validators
{
    public class InvoiceItemValidator : AbstractValidator<InvoiceItemModel>
    {
        public InvoiceItemValidator()
        {
            RuleFor(x => x.InvoiceId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .NotEqual(0).WithMessage("{PropertyName} of Phone Number cannot be Zero");

            RuleFor(x => x.Product.Id)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .NotEqual(0).WithMessage("{PropertyName} of Phone Number cannot be Zero");

            RuleFor(x => x.BuyPrice).GreaterThan(-1).WithMessage("{PropertyName} must be >= 0");

            RuleFor(x => x.SellPrice).GreaterThan(-1).WithMessage("{PropertyName} must be >= 0");

            RuleFor(x => x.CountString)
                .MaximumLength(50).WithMessage("{PropertyName} cannot be more than 50 characters ({TotalLength})");
            RuleFor(x => x.IsCountStringValid).Equal(true).WithMessage("Count is not valid");

            //item.RuleFor(x => x.DateCreated)
            //    .Cascade(CascadeMode.Stop)
            //    .NotEmpty().WithMessage("{PropertyName} cannot be empty")
            //    .Matches(@"([12]\d{3}\/(0[1-9]|1[0-2])\/(0[1-9]|[12]\d|3[01]))$").WithMessage("{PropertyName} is not valid")
            //    .Length(10).WithMessage("{PropertyName} cannot be more than 10 characters ({TotalLength})");

            //item.RuleFor(x => x.TimeCreated)
            //    .Cascade(CascadeMode.Stop)
            //    .NotEmpty().WithMessage("{PropertyName} cannot be empty")
            //    .Matches(@"^(?:(?:([01]?\d|2[0-3]):)?([0-5]?\d):)?([0-5]?\d)$").WithMessage("{PropertyName} is not valid")
            //    .Length(8).WithMessage("{PropertyName} cannot be more than 8 characters ({TotalLength})");

            //item.RuleFor(x => x.DateUpdated)
            //    .Cascade(CascadeMode.Stop)
            //    .Matches(@"([12]\d{3}\/(0[1-9]|1[0-2])\/(0[1-9]|[12]\d|3[01]))$").WithMessage("{PropertyName} is not valid")
            //    .Length(10).WithMessage("{PropertyName} cannot be more than 10 characters ({TotalLength})");

            //item.RuleFor(x => x.TimeUpdated)
            //    .Cascade(CascadeMode.Stop)
            //    .Matches(@"^(?:(?:([01]?\d|2[0-3]):)?([0-5]?\d):)?([0-5]?\d)$").WithMessage("{PropertyName} is not valid")
            //    .Length(8).WithMessage("{PropertyName} cannot be more than 8 characters ({TotalLength})");

            RuleFor(x => x.Delivered).NotEmpty().WithMessage("{PropertyName} cannot be empty");
            RuleFor(x => x.Descriptions).MaximumLength(50).WithMessage("{PropertyName} cannot be more than 50 characters ({TotalLength})");
        }
    }
}