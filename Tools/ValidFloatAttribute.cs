using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class ValidFloatAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Value is required.");
            }

            if (float.TryParse(value.ToString(), out _))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("TotalPrice description must be a valid float number.");
            }
        }
    }
}
