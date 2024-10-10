using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingServices.Data;
using CusromerProject.DTO.Payment;
using Microsoft.AspNetCore.Authorization;
using CustomerProject.DTO.Payment;

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
        //[Authorize]
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

        [HttpGet("PaymentGetways")]
        //[Authorize]
        public async Task<ActionResult<IEnumerable<PaymentIncome>>> GetPaymentIncome()
        {
            try
            {

                var PaymentIncomes = await _context.PaymentIncomes
                    .Include(p => p.Bookings)
                    .Include(p => p.Discounts)
                    .Where(p => p.IsBlocked == false)
                    .Select(paymentIncome => new PaymentIncomeDTO
                    {
                        Name = paymentIncome.Name,
                        Percentage = paymentIncome.Percentage,
                    }).ToListAsync();
                return Ok(PaymentIncomes);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }

        }

        //GET: api/Payments/5
        [HttpGet("{id}")]
        //[Authorize]
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
        //[Authorize]
        public async Task<ActionResult<Payment>> PostPayment([Bind("CustomerId,PaymentValue,PaymentDate,BookingId")] PostedPaymentDTO postedPaymentDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); // Return 400 if the model state is invalid
                }
                var payment = new Payment()
                {
                    BookingId = postedPaymentDTO.BookingId,
                    CustomerId = postedPaymentDTO.CustomerId,
                    PaymentDate = postedPaymentDTO.PaymentDate,
                    PaymentValue = postedPaymentDTO.PaymentValue
                };
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPaymentByID), new { id = payment.PaymentId }, postedPaymentDTO); // Return 201 if successful
            }
            catch (DbUpdateException ex)
            {
                // Log the exception (you could use a logging framework like NLog or Serilog)
                // For now, we return a 500 Internal Server Error with a message
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error saving payment to the database: {ex.Message}");
            }
            catch (Exception ex)
            {
                // General catch block for any other exceptions
                return StatusCode(StatusCodes.Status500InternalServerError, $"An unexpected error occurred: {ex.Message}");
            }
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
