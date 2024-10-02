using BookingServices.Data;
using BookingServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace BookingServices.Controllers
{
    [Authorize(Roles = "Provider")]
    public class ProfileProviderController : Controller
    {
        ApplicationDbContext context;
        ErrorViewModel errorViewModel = new ErrorViewModel { Message = "", Controller = "", Action = "" };
        string UserID = "6BA8DE65-9B57-466B-87EE-3D3279CED4C6";
        private readonly UserManager<IdentityUser> _userManager;
        ProviderDataVM providerDataVM = new ProviderDataVM();
        private readonly SignInManager<IdentityUser> _signInManager;

        public ProfileProviderController([FromServices] ApplicationDbContext _context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            context = _context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            string userIdFromManager = user?.Id ?? "";
            var provider = context.ServiceProviders.Include(p => p.IdentityUser).FirstOrDefault(p => p.ProviderId == userIdFromManager);
            providerDataVM = new ProviderDataVM()
            {
                ProviderId = provider.ProviderId,
                Name = provider.Name,
                Email = provider.IdentityUser.Email,
                Phone = provider.IdentityUser.PhoneNumber,
                Rate = provider.Rate,
                Balance = provider.Balance,
                ReservedBalance = provider.ReservedBalance,
                ServiceDetails = provider.ServiceDetails
            };
            return View(providerDataVM);
        }
        [HttpPost, Route("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProviderDataVM providerDataVM)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                var provider = await context.ServiceProviders.FindAsync(providerDataVM.ProviderId);
                if (provider != null)
                {
                    provider.Name = providerDataVM.Name;
                    provider.ServiceDetails = providerDataVM.ServiceDetails;
                    user.PhoneNumber = providerDataVM.Phone;

                    context.ServiceProviders.Update(provider);
                    await _userManager.UpdateAsync(user);
                    await context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            return View(providerDataVM);
        }


        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View("ProviderResetPassword");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(Provider_ResetPassowrd model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
                if (!passwordCheck)
                {
                    ModelState.AddModelError(string.Empty, "Current password is incorrect.");
                    return View(model);
                }

                if (model.NewPassword != model.ConfirmNewPassword)
                {
                    ModelState.AddModelError(string.Empty, "The new password and confirmation password do not match.");
                    return View(model);
                }

                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Password changed successfully.";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
    }
}
