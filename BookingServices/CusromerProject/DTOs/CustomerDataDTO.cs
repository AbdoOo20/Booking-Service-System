using System.ComponentModel.DataAnnotations;

namespace CusromerProject.DTOs
{
    public class CustomerDataDTO
    {
        public string CustomerId { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string AlternativePhone { get; set; }
        public string SSN { get; set; }
        public string City { get; set; }
        public string Password { get; set; }
    }
}
