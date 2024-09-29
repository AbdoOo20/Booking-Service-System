using BookingServices.Data;
using BookingServices.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Contracts;

namespace BookingServices.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminHomeController : Controller
    {
        ApplicationDbContext context;
        private readonly UserManager<IdentityUser> _userManager;
        public AdminHomeController(ApplicationDbContext _context, UserManager<IdentityUser> userManager)
        {
            context = _context;
            _userManager = userManager;
        }
        public async Task<ActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            string userIdFromManager = user?.Id ?? "";
            var customers = context.Customers.Count();
            var providers = context.ServiceProviders.Count();
            var services = context.Services.Count();
            var servicesOffLine = context.Services.Where(s => s.IsOnlineOrOffline == false).Count();
            var servicesOnLine = context.Services.Where(s => s.IsOnlineOrOffline == true).Count();
            var totalMoneyService = context.BookingServices.Select(bs => bs.Booking).Sum(b => b.Price);
            HomeAdminVM homeAdminVM = new HomeAdminVM();
            homeAdminVM.CustomerCount = customers;
            homeAdminVM.ProvidersCount = providers;
            homeAdminVM.ServicesCount = services;
            homeAdminVM.ServicesOffLineCount = servicesOffLine;
            homeAdminVM.ServicesOnLineCount = servicesOnLine;
            homeAdminVM.TotalMoneyService = totalMoneyService;
            //Start Chart
            var salesData = new List<int> { providers , customers };
            ViewBag.SalesData = salesData;
            //End Chart
            return View(homeAdminVM);
        }
    }
}
