using FluentValidation;
using SharedLibrary.DalModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Validators
{
    public class ChequeValidator : AbstractValidator<ChequeModel>
    {
        public ChequeValidator()
        {
            RuleFor(c => c.Drawer)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty.")
                .MaximumLength(50).WithMessage("{PropertyName} cannot be more than 50 characters ({TotalLength})");

            RuleFor(c => c.Orderer)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty.")
                .MaximumLength(50).WithMessage("{PropertyName} cannot be more than 50 characters ({TotalLength})");

            RuleFor(c => c.PayAmount)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .GreaterThan(0).WithMessage("{PropertyName} must be >= 0");

            RuleFor(c => c.About)
                .MaximumLength(100).WithMessage("{PropertyName} cannot be more than 100 characters ({TotalLength})");

            RuleFor(c => c.IssueDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Matches(@"([12]\d{3}\/(0[1-9]|1[0-2])\/(0[1-9]|[12]\d|3[01]))$").WithMessage("{PropertyName} is not valid")
                .Length(10).WithMessage("{PropertyName} cannot be more than 10 characters ({TotalLength})");

            RuleFor(c => c.DueDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Matches(@"([12]\d{3}\/(0[1-9]|1[0-2])\/(0[1-9]|[12]\d|3[01]))$").WithMessage("{PropertyName} is not valid")
                .Must((model, field) => string.Compare(field.Replace("/", ""), model.IssueDate.Replace("/", "")) >= 0).WithMessage("تاریخ سررسید باید از تاریخ صدور بالاتر باشد")
                .Length(10).WithMessage("{PropertyName} cannot be more than 10 characters ({TotalLength})");

            RuleFor(c => c.BankName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty.")
                .MaximumLength(50).WithMessage("{PropertyName} cannot be more than 50 characters ({TotalLength})");

            RuleFor(c => c.Serial)
                .MaximumLength(25).WithMessage("{PropertyName} cannot be more than 25 characters ({TotalLength})");
            
            RuleFor(c => c.Identifier)
                .MaximumLength(20).WithMessage("{PropertyName} cannot be more than 20 digits ({TotalLength})");

            RuleForEach(c => c.Events).Cascade(CascadeMode.Stop).ChildRules(p =>
            {
                //p.RuleFor(x => x.ChequeId).NotEmpty().WithMessage("{PropertyName} cannot be empty")
                //    .NotEqual(0).WithMessage("{PropertyName} of Event cannot be Zero");

                p.RuleFor(x => x.EventDate).NotEmpty().WithMessage("{PropertyName} cannot be empty")
                    .Matches(@"([12]\d{3}\/(0[1-9]|1[0-2])\/(0[1-9]|[12]\d|3[01]))$").WithMessage("{PropertyName} is not valid")
                    .MaximumLength(10).WithMessage("{PropertyName} cannot be more than 10 characters ({TotalLength})");

                p.RuleFor(x => x.EventType).NotEmpty().WithMessage("{PropertyName} cannot be empty");

                p.RuleFor(x => x.EventText)
                    .MaximumLength(50).WithMessage("{PropertyName} cannot be more than 50 characters ({TotalLength})");
            });
        }
    }
}