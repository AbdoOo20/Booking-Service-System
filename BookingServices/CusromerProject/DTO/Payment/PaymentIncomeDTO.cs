using System.ComponentModel.DataAnnotations;

namespace CusromerProject.DTO.Payment
{
    public class PaymentIncomeDTO
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string? Name { get; set; }

        [Range(0.0, 20.0)]
        public decimal Percentage { get; set; }
    }
}
