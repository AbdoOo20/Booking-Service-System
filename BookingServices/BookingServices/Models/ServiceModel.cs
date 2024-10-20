using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookingServices.Models
{
    public class ServiceModel
    {
        [Display(Name = "Service ID")]
        public int ServiceId { get; set; }

        [Display(Name = "Name")]
        [RegularExpression(@"^[a-zA-Z\s]+$",ErrorMessage = "Name can only contain letters and spaces.")]
        [Required(ErrorMessage = "Please enter the name of the service.")]
        [StringLength(50, ErrorMessage = "The service name must be between 3 and 50 characters.", MinimumLength = 3)]
        public string? Name { get; set; }

        [Display(Name = "Details")]
        [Required(ErrorMessage = "Please provide details about the service.")]
        [MinLength(3, ErrorMessage = "Please write at least 3 charcters")]
        public string? Details { get; set; }

        [Display(Name = "Location")]
        [Required(ErrorMessage = "Please select the location of the service.")]
        public required string Location { get; set; }

        [Display(Name = "Start Time")]
        [Range(0, 22, ErrorMessage = "Start Time must be between 0 and 22 (24-hour format).")]
        [Required(ErrorMessage = "Please select the service start time.")]
        public int StartTime { get; set; }

        [Display(Name = "End Time")]
        [Range(1, 23, ErrorMessage = "End Time must be between 1 and 23 (24-hour format).")]
        [Required(ErrorMessage = "Please select the service end time.")]
        public int EndTime { get; set; }

        [Display(Name = "Quantity")]
        [Range(0, 10000, ErrorMessage = "Quantity must be between 0 and 10000.")]
        [Required]
        public int? Quantity { get; set; } = 0;

        [Required(ErrorMessage = "Please specify the deposit percentage.")]
        [Display(Name = "Deposit Percentage")]
        [Range(0.0, 20.0, ErrorMessage = "The deposit must be between 0% and 20%.")]
        public decimal InitialPaymentPercentage { get; set; } = 0;

        [Required(ErrorMessage = "Please provide the price of the service.")]
        [Display(Name = "Price")]
        [Range(100, 10000, ErrorMessage = "Price must be between 1 and 100000.")]
        public decimal ServicePrice { get; set; } = 0;

        [Display(Name = "Online / Offline")]
        public bool IsOnlineOrOffline { get; set; } = false;

        public bool IsRequestedOrNot { get; set; } = false;

        [Display(Name = "Category Name")]
        public string? CategoryName { get; set; }

        public int? CategoryId { get; set; }

        [Display(Name = "Base Service")]
        public int? BaseServiceId { get; set; }

        [Display(Name = "Provider Contract")]
        public int? ProviderContractId { get; set; }

        [Display(Name = "Admin Contract")]
        [Required(ErrorMessage = "Admin Contract is required.")]
        public int? AdminContractId { get; set; }

        [Display(Name = "Service Provider")]
        public string? ServiceProviderName { get; set; }

        public string? ServiceProviderId { get; set; }

        public bool IsBlocked { get; set; } = true;
    }
}
