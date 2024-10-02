using BookingServices.Data;
using BookingServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BookingServices.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PaymentReportsController : Controller
    {
        ApplicationDbContext _context;
        public List<PaymentReportViewModel> paymentIncomes;

        public PaymentReportsController(ApplicationDbContext context) 
        {
            _context = context;
        }

        public IActionResult Index(string? searchText, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var query = from B in _context.Bookings
                            join P in _context.PaymentIncomes
                            on B.PaymentIncomeId equals P.PaymentIncomeId
                            where B.Status == "Confirmed"
                            group B by new { P.PaymentIncomeId,P.Name, P.Percentage } into PaymentBooking
                            select new PaymentReportViewModel
                            {
                                PaymentIncomeId = PaymentBooking.Key.PaymentIncomeId,
                                Name = PaymentBooking.Key.Name,
                                PaymentCount = PaymentBooking.Count(),
                                TotalBenefit = PaymentBooking.Sum(pb => pb.Price * (pb.PaymentIncome.Percentage / 100)),
                                BookingDate = PaymentBooking.FirstOrDefault().BookDate
                            };

                // Apply filters
                if (!string.IsNullOrEmpty(searchText))
                {
                    query = query.Where(p => p.Name.Contains(searchText));
                }

                // Validate date range
                if (fromDate.HasValue && toDate.HasValue)
                {
                    if (fromDate.Value > toDate.Value)
                    {
                        ModelState.AddModelError("", "To date cannot be earlier than From date.");
                        // Optionally, you can return the view here to show the error message
                    }
                    else
                    {
                        query = query.Where(p => p.BookingDate >= fromDate.Value && p.BookingDate <= toDate.Value);
                    }
                }

                paymentIncomes = query.ToList();
                // Return JSON result if request is AJAX
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {                 
                    return Json(paymentIncomes);
                }

                return View(paymentIncomes);
            }
            catch (Exception ex)
            {
                ErrorViewModel e = new ErrorViewModel()
                {
                    Controller = nameof(PaymentReportsController),
                    Action = "Index",
                    Message = ex.Message
                };

                return View("Error", e);
            }
        }

        public IActionResult Details(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest("Invalid ID provided.");
                }

                var paymentDetails = _context.Bookings
                   .Join(_context.PaymentIncomes,
                       booking => booking.PaymentIncomeId,
                       paymentIncome => paymentIncome.PaymentIncomeId,
                       (booking, paymentIncome) => new { booking, paymentIncome })
                   .Where(x => x.paymentIncome.PaymentIncomeId == id)
                   .Join(_context.BookingServices,
                       x => x.booking.BookingId,
                       bookingService => bookingService.BookingId,
                       (x, bookingService) => new { x.booking, x.paymentIncome, bookingService })
                   .Join(_context.Services,
                       x => x.bookingService.ServiceId,
                       service => service.ServiceId,
                       (x, service) => new { x.booking, x.paymentIncome, x.bookingService, service })
                   .Join(_context.ServiceProviders,
                       x => x.service.ProviderId,
                       provider => provider.ProviderId,
                       (x, provider) => new { x.booking, x.paymentIncome, x.service, provider })
                   .Join(_context.Customers,
                       x => x.booking.CustomerId,
                       customer => customer.CustomerId,
                       (x, customer) => new PaymentReportDetailsViewModel
                       {
                           CustomerName = customer.Name,
                           ProviderName = x.provider.Name,
                           ServiceName = x.service.Name,
                           BookingData = x.booking.BookDate,
                           BookingPrice = x.booking.Price,
                           PaymentMethod = x.paymentIncome.Name
                       })
                   .ToList();

                if (paymentDetails == null || !paymentDetails.Any())
                {
                    return NotFound("No payment details found.");
                }

                return View(paymentDetails);
            }
            catch (InvalidOperationException ex)
            { 
                return BadRequest($"Database operation failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An internal server error occurred: {ex.Message}");
            }
        }

    }
}
