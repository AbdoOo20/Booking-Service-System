using BookingServices.Data;
using System.ComponentModel.DataAnnotations;

namespace CustomerProject.DTO.Customer
{
    public class UniqueSSNAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var context = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext));

            if (value == null)
            {
                return new ValidationResult("SSN Is Required");
            }

            string SSN = value.ToString();

            var customer = context.Customers.FirstOrDefault(c => c.SSN == SSN);
            if (customer != null)
            {
                return new ValidationResult("This SSN Already Exists");
            }

            return ValidationResult.Success;
        }
    }
}
