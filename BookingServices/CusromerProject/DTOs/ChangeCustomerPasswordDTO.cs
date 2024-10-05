using System.ComponentModel.DataAnnotations;

namespace CusromerProject.DTOs
{
    public class ChangeCustomerPasswordDTO
    {
        public string? CustomerId { get; set; }
        public string CurrentPassword { get; set; }
        [Display(Name = "New Password")] 
        public string NewPassword { get; set; }

        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; }
    }
}
