using BookingServices.Data;
using Microsoft.AspNetCore.Mvc;
using BookingServices.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;


namespace BookingServices.Controllers
{
    [Authorize(Roles = "Provider")]
    public class ProviderHomeController : Controller
    {
        ApplicationDbContext context;
        private readonly UserManager<IdentityUser> _userManager;
        public ProviderHomeController(ApplicationDbContext _context, UserManager<IdentityUser> userManager)
        {
            context = _context;
            _userManager = userManager;
        }
        public async Task<ActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            string userIdFromManager = user?.Id ?? "";
            var services = context.Services.Where(s => s.ProviderId == userIdFromManager).Count();
            var contracts = context.ProviderContracts.Where(pc => pc.ProviderId == userIdFromManager).Count();
            var totalBookingsForServiceProvider =
                context.BookingServices.
                Where(bs => bs.Service != null &&
                    bs.Service.ServiceProvider != null &&
                    bs.Service.ServiceProvider.ProviderId == userIdFromManager)
                .Count();
            var packageCreatedCount = context.Packages.Where(p => p.ProviderId == userIdFromManager).Count();
            var TotalOfClicks = context.Links.Where(l => l.ProviderId == userIdFromManager).Sum(l => l.NumberOfClicks);
            //var TotalMoneyService = context.BookingServices.Where(bs => bs.Service.ProviderId == userIdFromManager).
            //Select(bs => bs.Booking);
            var totalPaymentValue = context.BookingServices
            .Where(bs => bs.Service.ProviderId == userIdFromManager)
            .Join(context.Payments,
                  bs => bs.BookingId,
                  p => p.BookingId,
                  (bs, p) => p.PaymentValue) 
            .Sum();
            HomeInfoVM homeInfoVM = new HomeInfoVM();
            homeInfoVM.ServicesCount = services;
            homeInfoVM.ContaractCount = contracts;
            homeInfoVM.BookServiceCount = totalBookingsForServiceProvider;
            homeInfoVM.PackageCreateCount = packageCreatedCount;
            homeInfoVM.TotalOfClicks = TotalOfClicks;
            homeInfoVM.TotalMoneyService = totalPaymentValue;
            //Start Chart
            var salesData = new List<int> { services, contracts, totalBookingsForServiceProvider, packageCreatedCount };
            ViewBag.SalesData = salesData;
            //End Chart
            if (homeInfoVM == null)
                return NotFound();
            return View(homeInfoVM);
        }
    }
}
