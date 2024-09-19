using System;
using System.Collections.Generic;

namespace BookingServices.Data;

public partial class ServiceProvider
{
    public int ProviderId { get; set; }

    public string? ImgSsn { get; set; }

    public decimal? Balance { get; set; }

    public decimal? Rate { get; set; }

    public virtual ICollection<Package> Packages { get; set; } = new List<Package>();

    public virtual ICollection<ProviderContract> ProviderContracts { get; set; } = new List<ProviderContract>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
