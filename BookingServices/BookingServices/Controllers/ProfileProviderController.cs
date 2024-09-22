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

        public ProfileProviderController([FromServices] ApplicationDbContext _context)
        {
            context = _context;
        }

        public ActionResult Index()
        {
            var provider = context.ServiceProviders.Include(p => p.IdentityUser).FirstOrDefault(p => p.ProviderId == UserID);
            ProviderDataVM providerDataVM = new ProviderDataVM()
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

        // GET: ProfileProviderController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProfileProviderController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProfileProviderController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProfileProviderController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProfileProviderController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProfileProviderController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProfileProviderController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
