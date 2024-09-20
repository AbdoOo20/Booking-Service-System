using System;
using System.Collections.Generic;

namespace BookingServices.Data;

public partial class Payment
{
    public int PaymentId { get; set; }

    public string CustomerId { get; set; } = null!;

    public int BookingId { get; set; }

    public DateTime PaymentDate { get; set; }

    public decimal PaymentValue { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;
}
