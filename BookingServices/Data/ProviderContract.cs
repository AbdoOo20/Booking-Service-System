using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingServices.Data;

public partial class ProviderContract
{
    [Key]
    public int ContractId { get; set; }

    public string? Details { get; set; }
    [ForeignKey("Provider")]
    public string? ProviderId { get; set; }

    public virtual ServiceProvider? Provider { get; set; }

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
