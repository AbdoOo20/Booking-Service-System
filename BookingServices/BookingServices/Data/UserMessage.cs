using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingServices.Data
{
    public class UserMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MsgID { get; set; }

        [ForeignKey("Sender")]
        public string? SenderID { get; set; }

        [ForeignKey("Receiver")]
        public string? ReceiverID { get; set; }

        [Required]
        public string? MessageText { get; set; }

        public DateTime DateSent { get; set; } = DateTime.Now;


        public virtual IdentityUser? Sender { get; set; }
        
        public virtual IdentityUser? Receiver { get; set; }

    }
}
