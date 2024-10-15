using System.ComponentModel.DataAnnotations;

namespace CusromerProject.DTO.Account
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "User Name is required.")]
        [StringLength(100, ErrorMessage = "User Name cannot exceed 50 characters.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
