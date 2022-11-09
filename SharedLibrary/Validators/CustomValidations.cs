using DotNetStandardCalculator;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Validators
{
    public class CountStringIsValid : ValidationAttribute //For DataAnnotation
    {
        public override bool IsValid(object value)
        {
            var str = (string)value;
            return StandardCalculator.IsCalculatable(str);
        }
    }
}
