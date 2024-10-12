using BookingServices.Data;
using System.ComponentModel.DataAnnotations;

namespace CustomerProject.DTO.Customer
{
    public class UniquePhoneAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var context = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext));

            if (value == null)
            {
                return new ValidationResult("Phone Is Required");
            }

            string phone = value.ToString();

            var customer = context.Customers.FirstOrDefault(c => c.AlternativePhone == phone);
            if (customer != null)
            {
                return new ValidationResult("This AlternativePhone Already Exists");
            }

            return ValidationResult.Success;
        }
    }
}
