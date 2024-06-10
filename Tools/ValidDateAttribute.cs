using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class ValidDateAttribute: ValidationAttribute
    {
        private readonly string _dateFormat;

        public ValidDateAttribute(string dateFormat)
        {
            _dateFormat = dateFormat;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Date is required.");
            }

            if (DateTime.TryParseExact(value.ToString(), _dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult($"The field {validationContext.DisplayName} must be a valid date in the format {_dateFormat}.");
            }
        }
    }
}
