using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingServices.ViewModel
{
    public class OfflineCustomer
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"^[12]\d{9}$", ErrorMessage = "Please enter a valid Saudi SSN number.")]
        public string SSN { get; set; }

        [Required]
        [RegularExpression(@"^(009665|9665|\+9665|05|5)(5|0|3|6|4|9|1|8|7)([0-9]{7})$",
        ErrorMessage = "Please enter a valid Saudi phone number.")]
        public string Phone { get; set; }

        [Required]
        [RegularExpression(@"^(009665|9665|\+9665|05|5)(5|0|3|6|4|9|1|8|7)([0-9]{7})$",
        ErrorMessage = "Please enter a valid Saudi phone number.")]
        public string? AlternativePhone { get; set; }

        [Required(ErrorMessage = "Please select a City.")]
        public string City { get; set; }
    }
}
