using System;
using System.Collections.Generic;

namespace BookingServices.Data;

public partial class PaymentIncome
{
    public int PaymentIncomeId { get; set; }

    public string? Name { get; set; }

    public decimal? Percentage { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();
}
