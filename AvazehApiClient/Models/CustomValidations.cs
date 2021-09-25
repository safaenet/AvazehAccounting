using DotNetStandardCalculator;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvazehApiClient.Models
{
    public class CountStringIsValid : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var str = (string)value;
            try
            {
                _ = StandardCalculator.CalculateFromString(str);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
