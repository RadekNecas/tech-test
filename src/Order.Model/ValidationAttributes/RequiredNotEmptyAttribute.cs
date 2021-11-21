using System.ComponentModel.DataAnnotations;

namespace Order.Model.ValidationAttributes
{
    public class RequiredNotEmptyAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Value must be set.");
            }

            if (value is string str && string.IsNullOrEmpty(str))
            {
                return new ValidationResult("Value must be set. String value cannot be null or empty.");
            }

            return ValidationResult.Success;
        }
    }
}
