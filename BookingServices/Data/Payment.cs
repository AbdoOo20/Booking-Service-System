using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingServices.Data;

public partial class Payment
{
    [Key]
    public int PaymentId { get; set; }

    public string CustomerId { get; set; }

    public int BookingId { get; set; }

    public DateTime PaymentDate { get; set; }

    public decimal PaymentValue { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;
}
