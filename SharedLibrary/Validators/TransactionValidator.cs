using FluentValidation;
using SharedLibrary.DalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Validators
{
    public class TransactionValidator : AbstractValidator<TransactionModel>
    {
        public TransactionValidator()
        {
            RuleFor(x => x.FileName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .MaximumLength(100).WithMessage("{PropertyName} cannot be more than 100 characters ({TotalLength})");
            RuleFor(x => x.DateCreated).Matches(@"\d\d\d\d/\d\d/\d\d$").WithMessage("{PropertyName} not valid.");
            RuleFor(x => x.TimeCreated).Matches(@"\d\d:\d\d:\d\d$").WithMessage("{PropertyName} not valid.");
        }
    }
}
