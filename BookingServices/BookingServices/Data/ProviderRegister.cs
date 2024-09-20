﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingServices.Data
{
    public class ProviderRegister
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProviderRegisterId { get; set; }
        
        [StringLength(255)]
        public string? ProviderName { get; set; }

        [Required]
        [RegularExpression(@"%_@_%._%")]
        public string? ProviderEmail { get; set; }

        [Required]
        public string? ServiceDetails { get; set; }

        [Required]
        [RegularExpression(@"^(009665|9665|\+9665|05|5)(5|0|3|6|4|9|1|8|7)([0-9]{7})$")]
        public string? ProviderPhoneNumber { get; set; }

    }
}
