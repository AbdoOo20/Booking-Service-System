using CustomerProject.DTO.Customer;
using System.ComponentModel.DataAnnotations;

namespace CusromerProject.DTO.Customer
{
    public class ChangeCustomerPasswordDTO
    {
        [Required(ErrorMessage = "ID is Required")]
        public string CustomerId { get; set; }
        [Required(ErrorMessage = "Password Is Required")]
        [CheckPassword]
        public string CurrentPassword { get; set; }
        [Required(ErrorMessage = "Password Is Required")]
        [CheckPassword]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Password Is Required")]
        [CheckPassword]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; }
    }
}
