using System;
using System.Collections.Generic;

namespace BookingServices.Data;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string AlternativePhone { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
