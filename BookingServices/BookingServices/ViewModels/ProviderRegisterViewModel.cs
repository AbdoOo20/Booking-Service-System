﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingServices.ViewModels
{
    public class ProviderRegisterViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProviderRegisterId { get; set; }

        [Required, Display(Name = "Name")]
        public string? ProviderName { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email format.")]
        public string? ProviderEmail { get; set; }

        [Display(Name = "Details of service")]
        [Required(ErrorMessage = "Service details are required.")]
        [StringLength(500, ErrorMessage = "Details cannot exceed 500 characters.")]
        public string? ServiceDetails { get; set; }

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^(009665|9665|\+9665|05|5)(5|0|3|6|4|9|1|8|7)([0-9]{7})$", ErrorMessage = "Invalid phone number format.")]
        public string? ProviderPhoneNumber { get; set; }
    }
}
