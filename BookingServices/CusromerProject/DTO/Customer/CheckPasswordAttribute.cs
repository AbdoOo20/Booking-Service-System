using BookingServices.Data;
using System.ComponentModel.DataAnnotations;

namespace CustomerProject.DTO.Customer
{
    public class CheckPasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var context = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext));

            if (value == null)
            {
                return new ValidationResult("Password Is Required");
            }

            string Password = value.ToString();
            if (!(Password.Any(char.IsUpper)))
                return new ValidationResult("Password must have At least one uppercase letter");
            if (!(Password.Any(char.IsLower)))
                return new ValidationResult("Password must have At least one lowercase letter");
            if (!(Password.Any(char.IsDigit)))
                return new ValidationResult("Password must have At least one Number");
            if (!(Password.Any(ch => !char.IsLetterOrDigit(ch))))
                return new ValidationResult("Password must have At least one special character");
            if (!(Password.Length >= 6))
                return new ValidationResult("Password must have At least 6 characters long");
            return ValidationResult.Success;
        }
    }
}
