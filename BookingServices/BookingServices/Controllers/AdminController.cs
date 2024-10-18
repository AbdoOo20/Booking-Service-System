using BookingServices.Data;
using BookingServices.Models;
using BookingServices.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;


namespace BookingServices.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext _appcontext;
        ErrorViewModel errorViewModel = new ErrorViewModel { Message = "", Controller = "", Action = "" };


        public AdminController(ApplicationDbContext appcontext) {
            _appcontext = appcontext;
        }

        // to display all requested services
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var AllRequestedServices = await _appcontext.Services.Where(x => x.IsRequestedOrNot == true && x.IsOnlineOrOffline == false)
                    .Select(p => new ServicesVM
                    {
                        Id = p.ServiceId,
                        ServiceName = p.Name,
                        Location = p.Location,
                        Details = p.Details.Substring(0, 25) + "...",
                        ProviderName = p.ServiceProvider.Name
                    }).ToListAsync();

                return View(AllRequestedServices);
            }
            catch (Exception e)
            {
                errorViewModel = new ErrorViewModel { Message = "An unexpected error occurred. Please try again later.", Controller = " Admin", Action = "Index" };
                return View("Error", errorViewModel);
            }

        }

        // to display the sevice details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var _serviceDetails = await _appcontext.Services
                    .Where(s => s.ServiceId == id)
                    .Include(s => s.Category)
                    .Include(s => s.ServiceImages)
                    .Select(s => new ServiceDetails
                    {
                        Id = s.ServiceId,
                        ServiceName = s.Name,
                        Details = s.Details,
                        Category = s.Category != null ? s.Category.Name : "Uncategorized",
                        Location = s.Location,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        Quantity = s.Quantity,
                        ImageUrl = s.ServiceImages.Select(img => img.URL).ToList()
                    }).FirstOrDefaultAsync();

                if (_serviceDetails == null)
                {
                    errorViewModel = new ErrorViewModel { Message = "The requested Service was not found. Please try again later.", Controller = " Admin", Action = "Index" };
                    return View("Error", errorViewModel);
                }
                return View(_serviceDetails);
            }
            catch (Exception e)
            {
                errorViewModel = new ErrorViewModel { Message = "An unexpected error occurred. Please try again later.", Controller = " Admin", Action = "Index" };
                return View("Error", errorViewModel);
            }
        }

        // to accept the service
        [HttpGet]
        public async Task<IActionResult> Accept(int id)
        {
            try
            {
                var service = await _appcontext.Services.FirstOrDefaultAsync(s => s.ServiceId == id);
                if (service == null)
                {
                    errorViewModel = new ErrorViewModel { Message = "The requested Service was not found. Please try again later.", Controller = " Admin", Action = "Index" };
                    return View("Error", errorViewModel);
                }

                service.IsOnlineOrOffline = true;
                _appcontext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                errorViewModel = new ErrorViewModel { Message = "An unexpected error occurred. Please try again later.", Controller = " Admin", Action = "Index" };
                return View("Error", errorViewModel);
            }
        }

        // to reject the service
        [HttpGet]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                var service = await _appcontext.Services.FirstOrDefaultAsync(s => s.ServiceId == id);
                if (service == null)
                {
                    errorViewModel = new ErrorViewModel { Message = "The requested Service was not found. Please try again later.", Controller = " Admin", Action = "Index" };
                    return View("Error", errorViewModel);
                }

                service.IsRequestedOrNot = false;
                _appcontext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                errorViewModel = new ErrorViewModel { Message = "An unexpected error occurred. Please try again later.", Controller = " Admin", Action = "Index" };
                return View("Error", errorViewModel);
            }
        }


        [HttpGet]
        public async Task<IActionResult> ProviderBookingSummary()
        {
            try
            {
                var providerSummary = await _appcontext.ServiceProviders
                    .Select(provider => new
                    {
                        ProviderName = provider.Name,
                        TotalBookings = provider.Services
                                            .SelectMany(s => s.BookingServices)
                                            .Count(bs => bs.Booking.Status == "Accepted"),
                        TotalAmount = provider.Services
                                            .SelectMany(s => s.BookingServices)
                                            .Where(bs => bs.Booking.Status == "Accepted")
                                            .Sum(bs => bs.Booking.Price),
                        providerId = provider.ProviderId
                    }).ToListAsync();

                return View(providerSummary);
            }
            catch (Exception e)
            {
                errorViewModel = new ErrorViewModel { Message = "An unexpected error occurred. Please try again later.", Controller = " Admin", Action = "Index" };
                return View("Error", errorViewModel);
            }
        }

        [HttpGet]
        public IActionResult BookingDetails(string Id)
        {
            string userIdFromManager = Id;
            List<BookingViewModel> books = new List<BookingViewModel>();
            var bookings = (from b in _appcontext.Bookings
                            from bs in _appcontext.BookingServices
                            from s in _appcontext.Services
                            from c in _appcontext.Customers
                            from m in _appcontext.PaymentIncomes
                            where b.BookingId == bs.BookingId
                            && bs.ServiceId == s.ServiceId
                            && s.ProviderId == userIdFromManager && b.CustomerId == c.CustomerId && b.PaymentIncomeId == m.PaymentIncomeId
                            select new
                            {
                                BookingId = b.BookingId,
                                EventDate = b.EventDate,
                                Quantity = b.Quantity,
                                Price = b.Price,
                                BookDate = b.BookDate,
                                Status = b.Status,
                                Type = b.Type,
                                PaymentIncome = b.PaymentIncomeId != null ? m.Name : b.CashOrCashByHandOrInstallment,
                                CustomerName = c.Name,
                                ServiceName = s.Name,
                            });
            foreach (var item in bookings)
            {
                BookingViewModel newBook = new BookingViewModel
                {
                    BookingId = item.BookingId,
                    EventDate = item.EventDate,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    BookDate = item.BookDate,
                    Type = item.Type,
                    CustomerName = item.CustomerName,
                    PaymentIncome = item.PaymentIncome,
                    Status = item.Status,
                    ServiceName = item.ServiceName,
                };
                books.Add(newBook);
            }
            return View(books);
        }
    }
}
