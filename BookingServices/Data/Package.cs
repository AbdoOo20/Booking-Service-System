using System;
using System.Collections.Generic;

namespace BookingServices.Data;

public partial class Package
{
    public int PackageId { get; set; }

    public string PackageName { get; set; } = null!;

    public decimal Price { get; set; }

    public DateOnly? CreatedDate { get; set; }

    public string? PackageStatus { get; set; }

    public decimal PercentageForAdmin { get; set; }

    public int? ProviderId { get; set; }

    public virtual ServiceProvider? Provider { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
