using System;
using System.Collections.Generic;

namespace BookingServices.Data;

public partial class ServicePrice
{
    public int ServiceId { get; set; }

    public DateTime PriceDate { get; set; }

    public decimal Price { get; set; }

    public virtual Service Service { get; set; } = null!;
}
