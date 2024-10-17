using System.ComponentModel.DataAnnotations;

namespace CustomerProject.DTO.Customer
{
    public class bankAccountDTO
    {
        [Required(ErrorMessage = "Email Is Required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessage = "This InValid Email")]
        public required string bankAccount { get; set; }
    }
}
