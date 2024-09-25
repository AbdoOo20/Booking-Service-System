using BookingServices.Data;
using BookingServices.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BookingServices.Controllers
{
    [Authorize("ADMIN")]
    public class AdminController : Controller
    {
        private ApplicationDbContext _appcontext;
        public AdminController(ApplicationDbContext appcontext) {
            _appcontext = appcontext;
        }

        // to display all requested services
        public IActionResult Index()
        {
            var AllRequestedServices = _appcontext.Services.Where(x=>x.IsRequestedOrNot == true && x.IsOnlineOrOffline == false);
            var viewModel = AllRequestedServices.Select(p => new ServicesVM
            {
                Id = p.ServiceId,
                ServiceName = p.Name,
                Location = p.Location,
                Details = p.Details.Substring(0, 25) + "..."
                //ProvidorName = p.ServiceProvider.
            }).ToList();

            return View(viewModel);
        }

        // to display the sevice details
        public IActionResult Details(int id)
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

            if (_serviceDetails == null) return NotFound();
            return View(_serviceDetails);
        }
        
        // to accept the service
        public IActionResult Accept(int id)
        {
            var service = _appcontext.Services.FirstOrDefault(s => s.ServiceId == id);
            if (service == null) return NotFound();

            service.IsOnlineOrOffline = true;
            _appcontext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // to reject the service
        public IActionResult Reject(int id)
        {
            var service = _appcontext.Services.FirstOrDefault(s => s.ServiceId == id);
            if (service == null) return NotFound();

            service.IsRequestedOrNot = false;
            _appcontext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
