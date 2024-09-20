using System;
using System.Collections.Generic;

namespace BookingServices.Data;

public partial class ServiceProvider
{
    public string ProviderId { get; set; } = null!;

    public string? ImgSsn { get; set; }

    public decimal? Balance { get; set; }

    public decimal? Rate { get; set; }

    public string? ServiceDetails { get; set; }

    public virtual Link? Link { get; set; }

    public virtual ICollection<Package> Packages { get; set; } = new List<Package>();

    public virtual ICollection<ProviderContract> ProviderContracts { get; set; } = new List<ProviderContract>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
