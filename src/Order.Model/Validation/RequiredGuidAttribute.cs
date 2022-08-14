using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Order.Model.Validation
{
    /// <summary>
    /// validation annotation to validate Guid 
    /// </summary>
    public class RequiredGuidAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var input = Convert.ToString(value, CultureInfo.CurrentCulture);

            // let the Required attribute take care of this validation
            if (string.IsNullOrWhiteSpace(input))
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            // parse the input into Guid
            Guid guid;
            if (!Guid.TryParse(input, out guid))
            {
                // not a validstring representation of a guid
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return null;
        }


    }


}
