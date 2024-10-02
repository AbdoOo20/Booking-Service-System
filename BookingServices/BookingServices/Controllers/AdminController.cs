using BookingServices.Data;
using BookingServices.Models;
using BookingServices.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


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
        public IActionResult Index()
        {
            try
            {
                var AllRequestedServices = _appcontext.Services.Where(x => x.IsRequestedOrNot == true && x.IsOnlineOrOffline == false);
                var viewModel = AllRequestedServices.Select(p => new ServicesVM
                {
                    Id = p.ServiceId,
                    ServiceName = p.Name,
                    Location = p.Location,
                    Details = p.Details.Substring(0, 25) + "...",
                    ProviderName = p.ServiceProvider.Name
                }).ToList();
                return View(viewModel);
            }
            catch (Exception e)
            {
                errorViewModel = new ErrorViewModel { Message = "An unexpected error occurred. Please try again later.", Controller = " Admin", Action = "Index" };
                return View("Error", errorViewModel);
            }

        }

        // to display the sevice details
        [HttpGet]
        public IActionResult Details(int id)
        {
            try
            {
                var _serviceDetails = _appcontext.Services
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
                    }).FirstOrDefault();

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
        public IActionResult Accept(int id)
        {
            try
            {
                var service = _appcontext.Services.FirstOrDefault(s => s.ServiceId == id);
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
        public IActionResult Reject(int id)
        {
            try
            {
                var service = _appcontext.Services.FirstOrDefault(s => s.ServiceId == id);
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
        public IActionResult ProviderBookingSummary()
        {
            try
            {
                var providerSummary = _appcontext.ServiceProviders
                    .Select(provider => new
                    {
                        ProviderName = provider.Name,
                        TotalBookings = provider.Services
                                            .SelectMany(s => s.BookingServices)
                                            .Count(bs => bs.Booking.Status == "Accepted"),
                        TotalAmount = provider.Services
                                            .SelectMany(s => s.BookingServices)
                                            .Where(bs => bs.Booking.Status == "Accepted")
                                            .Sum(bs => bs.Booking.Price)
                    })
                    .ToList();

                return View(providerSummary);
            }
            catch (Exception e)
            {
                errorViewModel = new ErrorViewModel { Message = "An unexpected error occurred. Please try again later.", Controller = " Admin", Action = "Index" };
                return View("Error", errorViewModel);
            }
        }
    }
}
