using BookingServices.Data;
using BookingServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookingServices.Controllers
{
    public class AdminServiceProviderController : Controller
    {
        ApplicationDbContext _context;

        public AdminServiceProviderController(ApplicationDbContext context) 
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var providers = _context.ServiceProviders.Include(p => p.IdentityUser).ToList();

            List<ProviderDataVM> providerDataVMs = new List<ProviderDataVM>();
            foreach (var p in providers)
            {
                var providerDataVM = new ProviderDataVM()
                {
                    Name = p.Name,
                    Phone = p.IdentityUser.PhoneNumber,
                    Email = p.IdentityUser.Email,
                    ServiceDetails = p.ServiceDetails,
                };
                providerDataVMs.Add(providerDataVM);
            }

            return View(providerDataVMs);
        }

        // GET: AdminServiceProvider/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminServiceProvider/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminServiceProvider/Create
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

        // GET: AdminServiceProvider/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdminServiceProvider/Edit/5
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

        // GET: AdminServiceProvider/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdminServiceProvider/Delete/5
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
