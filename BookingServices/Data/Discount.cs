using System;
using System.Collections.Generic;

namespace BookingServices.Data;

public partial class Discount
{
    public int DiscountId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? PaymentIncomeId { get; set; }

    public virtual PaymentIncome? PaymentIncome { get; set; }
}
