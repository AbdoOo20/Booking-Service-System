using System.ComponentModel.DataAnnotations;

namespace CusromerProject.DTO.Payment
{
    public class PaymentIncomeDTO
    {
        [StringLength(50)]
        public string? Name { get; set; }

        [Range(0.0, 20.0)]
        public decimal Percentage { get; set; }
        public bool? IsBlooked { get; set; }
    }
}
