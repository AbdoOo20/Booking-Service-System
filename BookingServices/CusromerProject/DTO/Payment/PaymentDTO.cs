using System.ComponentModel.DataAnnotations;

namespace CusromerProject.DTO.Payment
{
    public class PaymentDTO
    {
        public int paymentID { get; set; }

        public string? CustomerName { get; set; }

        public string? CustomerID { get; set; }


        public DateTime EventDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public string? Status { get; set; }

        public DateTime PaymentDate { get; set; }

        [Range(1.0, double.MaxValue)]
        public decimal PaymentValue { get; set; }

        public string? ServiceName { get; set; }

        public int BookingID { get; set; }




    }
}
