using System.ComponentModel.DataAnnotations;

namespace CusromerProject.DTO.Account
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
