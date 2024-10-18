using BookingServices.Data;
using BookingServices.Data.Migrations;
using BookingServices.Hubs;
using BookingServices.Models;
using BookingServices.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BookingServices.Controllers
{
    public class ProviderRegisterController : Controller
    {
        ApplicationDbContext _context;
        ErrorViewModel errorViewModel = new ErrorViewModel();
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHubContext<AdminNotificationHub> _hubContext;

        public ProviderRegisterController(ApplicationDbContext context, UserManager<IdentityUser> userManager , IHubContext<AdminNotificationHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var Providers = _context.ProviderRegisters.ToList();

            return View(Providers);
        }

        public IActionResult Create()
        {
            return View(new ProviderRegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProviderRegisterViewModel providerVM)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    var existingProvider = await  _userManager.FindByEmailAsync(providerVM.ProviderEmail);
                    var existingWaitingProvider = await _context.ProviderRegisters
                        .Where(p => p.ProviderEmail == providerVM.ProviderEmail).FirstOrDefaultAsync();
                    if (existingWaitingProvider != null || existingProvider != null && await _userManager.IsInRoleAsync(existingProvider, "Provider"))
                    {
                        ModelState.AddModelError("ProviderEmail", "The email address is already exists.");
                        return View(providerVM);
                    }

                    // Check if phone number already exists
                    var existingWaintingPhone = await _context.ProviderRegisters
                        .Where(p => p.ProviderPhoneNumber == providerVM.ProviderPhoneNumber).FirstOrDefaultAsync();

                    var existingPhone = await _userManager.Users
                        .Where(u => u.PhoneNumber == providerVM.ProviderPhoneNumber)
                        .FirstOrDefaultAsync();

                    if (existingWaintingPhone != null || existingPhone != null)
                    {
                        ModelState.AddModelError("ProviderPhoneNumber", "This phone number already exists.");
                        return View(providerVM);
                    }

                    ProviderRegister providerRegister = new ProviderRegister
                    {
                        ProviderName = providerVM.ProviderName,
                        ProviderPhoneNumber = providerVM.ProviderPhoneNumber,
                        ProviderEmail = providerVM.ProviderEmail,
                        ServiceDetails = providerVM.ServiceDetails,
                    };
                    await _context.ProviderRegisters.AddAsync(providerRegister);
                    TempData["SuccessMessage"] = "Your data has been sent successfully!";

                    // Hub Code
                    NotificationAdmin notification = new NotificationAdmin
                    {
                        NotificationTitle = $"The Provider {providerVM.ProviderName} Request To Join Our Site" , 
                        Time = DateTime.Now
                    };

                    await _context.NotificationAdmins.AddAsync(notification);

					await _context.SaveChangesAsync();
                    List<NotificationAdmin> notificationList = _context.NotificationAdmins.ToList();
                    var length = notificationList.Count();

                    await _hubContext.Clients.All.SendAsync("ReceiveMessage", $"The Provider {providerVM.ProviderName} Request To Join Our Site", DateTime.Now , length);



                    return RedirectToAction("Create");
                }

                return View(providerVM);
            }
            catch
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again later.";

                return View(providerVM);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task <IActionResult> Delete(int id) 
        {
            if(id == 0)
            {
                errorViewModel.Message = $"An error occurred!";
                errorViewModel.Controller = "ProviderRegister";
                errorViewModel.Action = nameof(Index);

                return View("Error", errorViewModel);
            }

            ProviderRegister providerRegister =  await _context.ProviderRegisters.FindAsync(id);
            if(providerRegister == null)
            {
                errorViewModel.Message = $"Not Found!";
                errorViewModel.Controller = "ProviderRegister";
                errorViewModel.Action = nameof(Index);

                return View("Error", errorViewModel);
            }

            _context.ProviderRegisters.Remove(providerRegister);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
