using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookingServices.Models
{
    public class CustomerData
    {
        public string? CustomerId { get; set; }
        public required string Name { get; set; }
        [Display(Name = "Online/Offline")]
        public bool IsOnlineOrOfflineUser { get; set; }
        public string? AlternativePhone { get; set; }
        public string? Phone { get; set; }
        public required string SSN { get; set; }
        public required string City { get; set; }

        public bool? IsBlocked { get; set; }
    }
}
