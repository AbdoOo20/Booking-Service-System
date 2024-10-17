using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookingServices.Models
{
    public class ServiceModel
    {
        [Display(Name = "Service ID")]
        public int ServiceId { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Please enter the name of the service.")]
        [StringLength(50, ErrorMessage = "The service name must be between 3 and 50 characters.", MinimumLength = 3)]
        public string? Name { get; set; }

        [Display(Name = "Details")]
        [Required(ErrorMessage = "Please provide details about the service.")]
        public string? Details { get; set; }

        [Display(Name = "Location")]
        [Required(ErrorMessage = "Please select the location of the service.")]
        public required string Location { get; set; }

        [Display(Name = "Start Time")]
        [Range(0, 23, ErrorMessage = "Start Time must be between 0 and 23 (24-hour format).")]
        [Required(ErrorMessage = "Please select the service start time.")]
        public int StartTime { get; set; }

        [Display(Name = "End Time")]
        [Range(1, 24, ErrorMessage = "End Time must be between 1 and 24 (24-hour format).")]
        [Required(ErrorMessage = "Please select the service end time.")]
        public int EndTime { get; set; }

        [Display(Name = "Quantity")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative number.")]
        public int? Quantity { get; set; } = 0;

        [Required(ErrorMessage = "Please specify the deposit percentage.")]
        [Display(Name = "Deposit Percentage")]
        [Range(0.0, 20.0, ErrorMessage = "The deposit must be between 0% and 20%.")]
        public decimal InitialPaymentPercentage { get; set; } = 0;

        [Required(ErrorMessage = "Please provide the price of the service.")]
        [Display(Name = "Price")]
        [Range(1.00, (double)decimal.MaxValue, ErrorMessage = "The service price must be greater than 0.")]
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
        public int? AdminContractId { get; set; }

        [Display(Name = "Service Provider")]
        public string? ServiceProviderName { get; set; }

        public string? ServiceProviderId { get; set; }
    }
}
