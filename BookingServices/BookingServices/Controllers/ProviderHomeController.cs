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
            var totalPaymentValue = context.Bookings
                .Join(context.BookingServices, b => b.BookingId, bs => bs.BookingId, (b, bs) => new { b, bs })
                .Join(context.Services, b_bs => b_bs.bs.ServiceId, s => s.ServiceId, (b_bs, s) => new { b_bs.b, s })
                .Join(context.PaymentIncomes, b_s => b_s.b.PaymentIncomeId, p => p.PaymentIncomeId, (b_s, p) => new { b_s.b, b_s.s, p })
                .Where(joined => joined.s.ProviderId == userIdFromManager && joined.b.Status == "Confirmed").Select(joined => joined.b.Price - (joined.b.Price * joined.p.Percentage / 100))
                .Sum();
           string formattedValue = totalPaymentValue.ToString("F2");

            HomeInfoVM homeInfoVM = new HomeInfoVM();
            homeInfoVM.ServicesCount = services;
            homeInfoVM.ContaractCount = contracts;
            homeInfoVM.BookServiceCount = totalBookingsForServiceProvider;
            homeInfoVM.PackageCreateCount = packageCreatedCount;
            homeInfoVM.TotalOfClicks = TotalOfClicks;
            homeInfoVM.TotalMoneyService = formattedValue;
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
