using DotNetStandardCalculator;
using System.ComponentModel.DataAnnotations;

namespace SharedLibrary.Validators;

public class CountStringIsValid : ValidationAttribute //For DataAnnotation
{
    public override bool IsValid(object value)
    {
        var str = (string)value;
        return StandardCalculator.IsCalculatable(str);
    }
}