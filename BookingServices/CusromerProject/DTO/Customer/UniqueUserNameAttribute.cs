using BookingServices.Data;
using System.ComponentModel.DataAnnotations;

namespace CustomerProject.DTO.Customer
{
    public class UniqueUserNameAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var context = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext));

            if (value == null)
            {
                return new ValidationResult("UserName Is Required");
            }

            string userName = value.ToString();

            var customer = context.Customers.FirstOrDefault(c => c.IdentityUser.UserName == userName);
            if (customer != null)
            {
                return new ValidationResult("This UserName Already Exists");
            }

            return ValidationResult.Success;
        }
    }
}
