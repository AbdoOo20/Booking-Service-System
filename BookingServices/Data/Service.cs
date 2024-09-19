using System;
using System.Collections.Generic;

namespace BookingServices.Data;

public partial class Service
{
    public int ServiceId { get; set; }

    public string Name { get; set; } = null!;

    public string Details { get; set; } = null!;

    public string Location { get; set; } = null!;

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int? Quantity { get; set; }

    public decimal? InitialPaymentPercentage { get; set; }

    public int? CategoryId { get; set; }

    public int? ParentserviceId { get; set; }

    public int? ProviderContractId { get; set; }

    public int? AdminContractId { get; set; }

    public int? ProviderId { get; set; }

    public virtual AdminContract? AdminContract { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Service> InverseParentservice { get; set; } = new List<Service>();

    public virtual Service? Parentservice { get; set; }

    public virtual ServiceProvider? Provider { get; set; }

    public virtual ProviderContract? ProviderContract { get; set; }

    public virtual ICollection<ServicePrice> ServicePrices { get; set; } = new List<ServicePrice>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Package> Packages { get; set; } = new List<Package>();
}
