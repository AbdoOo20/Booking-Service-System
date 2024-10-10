using BookingServices.Data;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CustomerProject.DTO.Services
{
    public class ProviderDTO
    {
        public string? ProviderId { get; set; }
        public required string Name { get; set; }
        public string? ServiceDetails { get; set; }
        public decimal Rate { get; set; }
    }
}
