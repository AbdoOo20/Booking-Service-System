using System.ComponentModel.DataAnnotations;

namespace BookingServices.Models
{
    public class PaymentMethodDTO
    {
        public int PaymentIncomeId { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Percentage is required.")]
        [Range(1.0, 20.0, ErrorMessage = "Percentage must be between 1 and 20.")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Percentage must be a valid numeric value.")]
        public decimal Percentage { get; set; }


        public bool? IsBlocked { get; set; } = false;
    }
}
