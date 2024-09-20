using System;
using System.Collections.Generic;

namespace BookingServices.Data;

public partial class Review
{
    public string CustomerId { get; set; }

    public int BookingId { get; set; }

    public decimal Rating { get; set; }

    public string CustomerComment { get; set; } = null!;

    public DateTime CustomerCommentDate { get; set; }

    public string? ProviderReplayComment { get; set; }

    public DateTime ProviderReplayCommentDate { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;
}
