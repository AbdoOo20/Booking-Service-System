using BookingServices.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            return View(_context.ServiceProviders.ToList());
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
