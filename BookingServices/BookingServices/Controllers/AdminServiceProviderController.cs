using BookingServices.Data;
using BookingServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Numerics;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookingServices.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminServiceProviderController : Controller
    {
        ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminServiceProviderController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager; 
        }
          [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            // Fetch providers and services data
            var providers = await (from a in _context.Users
                                   join sp in _context.ServiceProviders on a.Id equals sp.ProviderId
                                   select new
                                   {
                                       providerID = sp.ProviderId,
                                       providerEmail = a.Email,
                                       providerPhoneNumber = a.PhoneNumber,
                                       providerName = sp.Name,
                                       providerBalance = sp.Balance,
                                       providerRate = sp.Rate,
                                       providerReservedBalance = sp.ReservedBalance,
                                       Isblocked = sp.IsBlooked
                                   }).ToListAsync();

            var numberOfServicesPerProvider = await _context.ServiceProviders
                .Select(sp => new
                {
                    ServiceProviderId = sp.ProviderId,
                    NumberOfServices = sp.Services.Count()
                })
                .ToListAsync();

            List<ProviderDataVM> providerDataVMs = new List<ProviderDataVM>();

            foreach (var p in providers)
            {
               
                var numberOfServices = numberOfServicesPerProvider
                    .FirstOrDefault(ns => ns.ServiceProviderId == p.providerID)?.NumberOfServices ?? 0;

                var providerDataVM = new ProviderDataVM()
                {
                    ProviderId = p.providerID,
                    Name = p.providerName,
                    Email = p.providerEmail,
                    Phone = p.providerPhoneNumber,
                    Rate = p.providerRate,
                    Balance = p.providerBalance,
                    ReservedBalance = p.providerReservedBalance,
                    NumberOfServices = numberOfServices ,
                    Isblocked = p.Isblocked
                };

                providerDataVMs.Add(providerDataVM);
            }


            return View(providerDataVMs);
        }
        [Authorize(Roles = "Admin")]
        // GET: AdminServiceProvider/Details/5
        public async Task<ActionResult> Details(string id)

        {
            var provider = await (from a in _context.Users
                                  join sp in _context.ServiceProviders on
                                  a.Id equals sp.ProviderId
                                  where sp.ProviderId == id
                                  select new
                                  {
                                      providerID = sp.ProviderId,
                                      providerEmail = a.Email,
                                      providerPhoneNumber = a.PhoneNumber,
                                      providerName = sp.Name,
                                      providerBalance = sp.Balance,
                                      providerRate = sp.Rate,
                                      providerReservedBalance = sp.ReservedBalance
                                  }).FirstOrDefaultAsync();

            var numberOfServices = await (from sp in _context.ServiceProviders
                                          join s in _context.Services on
                                          sp.ProviderId equals s.ProviderId
                                          select s.ServiceId
                                           ).CountAsync();

            ProviderDataVM providerDataVM = new ProviderDataVM()
            {
                ProviderId = provider.providerID,
                Name = provider.providerName,
                Email = provider.providerEmail,
                Phone = provider.providerPhoneNumber,
                Rate = provider.providerRate,
                Balance = provider.providerBalance,
                ReservedBalance = provider.providerReservedBalance,
                NumberOfServices = numberOfServices

            };
            return View(providerDataVM);
        }

        // GET: AdminServiceProvider/Create
        /* public ActionResult Create()
         {
             return View();
         }

         // POST: AdminServiceProvider/Create
         [HttpPost]
         [ValidateAntiForgeryToken]
         public ActionResult Create(IFormCollection collection)
         {

         }*/

        // GET: AdminServiceProvider/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return View("Error");
            }

            var provider = await (from a in _context.Users
                                  join sp in _context.ServiceProviders on
                                  a.Id equals sp.ProviderId
                                  where sp.ProviderId == id
                                  select new
                                  {
                                      ProviderId = sp.ProviderId,
                                      ProviderEmail = a.Email
                                  }).FirstOrDefaultAsync();

            if (provider == null)
            {
                return NotFound();
            }

         
            var model = new EditProviderViewModel
            {
                ProviderId = provider.ProviderId,
                Email = provider.ProviderEmail
            };

            return View(model);
        }


        // POST: AdminServiceProvider/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, EditProviderViewModel model)
        {
            if (id != model.ProviderId)
            {
                return View("Error");
            }

            if (ModelState.IsValid)
            {
                // Find the user by their ID
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                // Update email if it has changed
                user.Email = model.Email;

                // Reset password if a new one is provided
                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                    // Handle errors during password reset
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(model);  // Return the model to show validation errors
                    }
                }

                // Update the user in the database
                await _userManager.UpdateAsync(user);

                // Redirect to the Index page after successful update
                return RedirectToAction(nameof(Index));
            }

            // Return the view with the current model if the state is invalid
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> SearchProviders(string search)
        {
            var providers = await (from a in _context.Users
                                   join sp in _context.ServiceProviders on a.Id equals sp.ProviderId
                                   where sp.Name.Contains(search) || a.Email.Contains(search) || a.PhoneNumber.Contains(search)
                                   select new ProviderDataVM
                                   {
                                       ProviderId = sp.ProviderId,
                                       Name = sp.Name,
                                       Email = a.Email,
                                       Phone = a.PhoneNumber,
                                       Rate = sp.Rate,
                                       Balance = sp.Balance,
                                       ReservedBalance = sp.ReservedBalance,
                                       NumberOfServices = (from s in _context.Services where s.ProviderId == sp.ProviderId select s).Count()
                                   }).ToListAsync();

            return PartialView("_ProviderTablePartial", providers);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ToggleBlock(string id)
        {
            var provider = await _context.ServiceProviders.FindAsync(id);
            if (provider == null)
            {
                return Json(new { success = false, message = "Provider not found" });
            }

            provider.IsBlooked = !provider.IsBlooked;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isBlocked = provider.IsBlooked });
        }


    }
}
