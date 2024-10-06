using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingServices.Data;
using CusromerProject.DTO.Payment;

namespace CusromerProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PaymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Payments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPayments()
        {
            try
            {
                var payments = await _context.Payments
                     .Include(p => p.Customer)
                     .Include(p => p.Booking)
                     .Select(payment => new PaymentDTO
                     {
                         paymentID = payment.PaymentId,
                         PaymentValue = payment.PaymentValue,
                         CustomerName = payment.Customer.Name,
                         CustomerID= payment.CustomerId,
                         StartTime = payment.Booking.StartTime,
                         BookingID = payment.BookingId, 
                         EndTime = payment.Booking.EndTime,
                         EventDate = payment.Booking.EventDate,
                         PaymentDate = payment.PaymentDate,
                         Status = payment.Booking.Status,
                         ServiceName = (from bs in _context.BookingServices
                                        join s in _context.Services on bs.ServiceId equals s.ServiceId
                                        where bs.BookingId == payment.BookingId
                                        select s.Name).FirstOrDefault() 
                     })
                     .ToListAsync();


                
                return Ok(payments);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentIncome>>> GetPaymentIncome()
        {
            try {

                var PaymentIncomes = await _context.PaymentIncomes
                    .Include(p => p.Bookings)
                    .Include(p => p.Discounts)
                    .Select(paymentIncome => new PaymentIncomeDTO
                    {
                        Name = paymentIncome.Name,
                        Percentage= paymentIncome.Percentage,
                        IsBlooked= paymentIncome.IsBlooked
                    }).ToListAsync();
                return Ok(PaymentIncomes);
            }
             catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }

        }
        // GET: api/Payments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Payment>> GetPaymentByID(int id)
        {
            var payment = await _context.Payments
                                .Include(p => p.Customer)
                                .Include(p => p.Booking)
                                .Select(p => new PaymentDTO
                                {
                                    paymentID = p.PaymentId,
                                    PaymentValue = p.PaymentValue,
                                    CustomerName = p.Customer.Name,
                                    CustomerID = p.CustomerId,
                                    StartTime = p.Booking.StartTime,
                                    BookingID = p.BookingId,
                                    EndTime = p.Booking.EndTime,
                                    EventDate = p.Booking.EventDate,
                                    PaymentDate = p.PaymentDate,
                                    Status = p.Booking.Status,
                                    ServiceName = (from bs in _context.BookingServices
                                                   join s in _context.Services on bs.ServiceId equals s.ServiceId
                                                   where bs.BookingId == p.BookingId
                                                   select s.Name).FirstOrDefault()
                                })
                                .FirstOrDefaultAsync(p => p.paymentID == id);

            if (payment == null)
            {
                return NotFound();
            }

            return Ok(payment);
        }

        

        // POST: api/Payments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Payment>> PostPayment(PaymentDTO paymentDTO)
        {
            var payment = new Payment
            {
                CustomerId= paymentDTO.CustomerID,
                PaymentValue = paymentDTO.PaymentValue,
                PaymentDate = paymentDTO.PaymentDate,
                BookingId = paymentDTO.BookingID

            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPaymentByID), new { id = payment.PaymentId }, paymentDTO);
        }

      /*  // DELETE: api/Payments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();

            return NoContent();
        }*/

       private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.PaymentId == id);
        }


    }
}
