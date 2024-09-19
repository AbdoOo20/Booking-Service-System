using System;
using System.Collections.Generic;

namespace BookingServices.Data;

public partial class AdminContract
{
    public int ContractId { get; set; }

    public int? UserId { get; set; }

    public string? Details { get; set; }

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
