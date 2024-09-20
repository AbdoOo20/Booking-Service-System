using System;
using System.Collections.Generic;

namespace BookingServices.Data;

public partial class ProviderContract
{
    public int ContractId { get; set; }

    public string? Details { get; set; }

    public string? ProviderId { get; set; }

    public virtual ServiceProvider? Provider { get; set; }

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
