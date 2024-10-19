using System.ComponentModel.DataAnnotations;

namespace BookingServices.Models
{
    public class ProviderDataVM
    {
        public string? ProviderId { get; set; }

        public decimal Balance { get; set; }

        public decimal ReservedBalance { get; set; }
        
        public int NumberOfServices { get; set; } 
        public string? ServiceDetails { get; set; }

        public decimal Rate { get; set; }

        [Required, Display(Name = "Name")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 character")]
        [RegularExpression(@"^[a-zA-Z\s]+$",ErrorMessage = "Name can only contain letters and spaces.")]
        public string? Name { get; set; }

        public string? Email { get; set; }

        [Display(Name = "Bank Account")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.com$", ErrorMessage = "Invalid email format. It should be like (example@gmail.com)")]
        public string? BankAccount { get; set; }

        [Required]
        [RegularExpression(@"^(009665|9665|\+9665|05|5)(5|0|3|6|4|9|1|8|7)([0-9]{7})$",
        ErrorMessage = "Please enter a valid Saudi phone number.")]
        public string? Phone { get; set; }

        public bool? Isblocked { get; set; }
    }
}
