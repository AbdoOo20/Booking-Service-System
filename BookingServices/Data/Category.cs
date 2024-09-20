using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingServices.Data;

public partial class Category
{
    [Key]
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
