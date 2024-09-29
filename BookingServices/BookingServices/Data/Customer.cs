using BookingServices.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingServices.Data
{
    public class Customer
    {
        [Key]
        [ForeignKey("IdentityUser")]
        public string? CustomerId { get; set; }

        [Required, Display(Name = "Name")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 character")]
        public required string Name { get; set; }

        [Display(Name = "Is Online")]
        public bool IsOnlineOrOfflineUser { get; set; }

        [Required, Display(Name = "Alternative Phone")]
        [RegularExpression(@"^(009665|9665|\+9665|05|5)(5|0|3|6|4|9|1|8|7)([0-9]{7})$",
        ErrorMessage = "Please enter a valid Saudi phone number.")]
        public string? AlternativePhone { get; set; }

        [Required]
        [RegularExpression(@"^[12]\d{9}$")]
        public required string SSN { get; set; }

        [Required]
        public required string City { get; set; }

        public virtual IdentityUser? IdentityUser { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
