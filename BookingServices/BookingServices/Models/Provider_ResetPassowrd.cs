using System.ComponentModel.DataAnnotations;

namespace BookingServices.Models
{
    public class Provider_ResetPassowrd
    {

        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        [Required(ErrorMessage = "Current Password Is Required")]
        [CheckPassword]
        public string CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [Required(ErrorMessage = "Password Is Required")]
        [CheckPassword]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [CheckPassword]
        [Required(ErrorMessage = "Password Is Required")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; }
    }
}
