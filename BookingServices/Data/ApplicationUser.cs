using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookingServices.Data
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [RegularExpression(@"^[12]\d{9}$", ErrorMessage = "Invalid SSN format.")]
        public int SSN { get; set; }
        [Required]
        public string Location { get; set; }

        public virtual ServiceProvider ServiceProvider {get; set;}

        public virtual Customer Customer { get; set;}
    }
}
