using BookingServices.Data;
using BookingServices.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingServices.Controllers
{
    public class ProfileProviderController : Controller
    {
        ApplicationDbContext context;
        ErrorViewModel errorViewModel = new ErrorViewModel { Message = "", Controller = "", Action = "" };
        string UserID = "6BA8DE65-9B57-466B-87EE-3D3279CED4C6";
        private readonly UserManager<IdentityUser> _userManager;
        ProviderDataVM providerDataVM = new ProviderDataVM();

        public ProfileProviderController([FromServices] ApplicationDbContext _context, UserManager<IdentityUser> userManager)
        {
            context = _context;
            _userManager = userManager;
        }

        public async Task<ActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User); 
            string userIdFromManager = user?.Id ?? "";
            var provider = context.ServiceProviders.Include(p => p.IdentityUser).FirstOrDefault(p => p.ProviderId == UserID);
            providerDataVM = new ProviderDataVM()
            {
                ProviderId = provider.ProviderId,
                Name = provider.IdentityUser.UserName,
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
        public ActionResult Edit(ProviderDataVM providerDataVM)
        {
            //var user = await _userManager.GetUserAsync(User);
            //string userIdFromManager = user?.Id ?? "";
            if (ModelState.IsValid)
            {
                //user.UserName = providerDataVM.Name;
                //user.PhoneNumber = providerDataVM.Phone;
                var model = context.ServiceProviders.Find(providerDataVM.ProviderId);
                if (model == null) 
                {
                    errorViewModel = new ErrorViewModel { Message = "No Provider With This Data", Controller = "ProfileProvider", Action = "Index" };
                    return View("Error", errorViewModel);
                }
                model.ServiceDetails = providerDataVM.ServiceDetails;
                try
                {
                    context.ServiceProviders.Update(model);
                    //var result = await _userManager.UpdateAsync(user);
                    context.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    errorViewModel = new ErrorViewModel { Message = e.Message, Controller = "ProfileProvider", Action = "Index" };
                    return View("Error", errorViewModel);
                }
            }
            return RedirectToAction("Index");
        }
    }
}
