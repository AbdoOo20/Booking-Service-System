using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookingServices.Data;

public partial class Customer
{
    [Key]
    [ForeignKey("ApplicationUser")]
    public string CustomerId { get; set; }

    public string AlternativePhone { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ApplicationUser ApplicationUser { get; set; }
}
