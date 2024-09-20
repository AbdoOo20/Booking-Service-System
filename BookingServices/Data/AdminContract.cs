using System;
using System.Collections.Generic;

namespace BookingServices.Data;

public partial class AdminContract
{
    public int ContractId { get; set; }

    public string? UserId { get; set; }

    public string? Details { get; set; }
}
