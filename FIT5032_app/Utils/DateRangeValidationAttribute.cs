using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FIT5032_app.Utils
{   //A customized date validation class. The date of event must be greater than the current date time and less than a future time.
    public class DateRangeValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            value = (DateTime)value;
            var endDate = new DateTime(2029,12,31);
            
            if (DateTime.Now.CompareTo(value) <= 0 && endDate.CompareTo(value) >= 0)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Sorry, your selected date must be between current date time and 2029-12-31.");
            }
        }
    }
}