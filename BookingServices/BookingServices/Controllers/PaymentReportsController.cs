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

        public ActionResult Index(string? searchText, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var query = from B in _context.Bookings
                            join P in _context.PaymentIncomes
                            on B.PaymentIncomeId equals P.PaymentIncomeId
                            where B.Status == "Confirmed"
                            group B by new { P.Name, P.Percentage } into PaymentBooking
                            select new PaymentReportViewModel
                            {
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
    }
}
