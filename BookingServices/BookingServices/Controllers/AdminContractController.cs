using BookingServices.Data;
using BookingServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingServices.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminContractController : Controller
    {
        ApplicationDbContext context;
        ErrorViewModel errorViewModel = new ErrorViewModel { Message = "", Controller = "", Action = "" };

        public AdminContractController([FromServices] ApplicationDbContext _context)
        {
            context = _context;
        }

        public ActionResult Index()
        {
            return View(context.AdminContracts.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AdminContract adminContract)
        {
            if (ModelState.IsValid)
            {
                context.AdminContracts.Add(adminContract);
                try
                {
                    context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    errorViewModel = new ErrorViewModel { Message = e.Message, Controller = "AdminContract", Action = "Index" };
                    return View("Error", errorViewModel);
                }
            }
            return View(adminContract);
        }

        public ActionResult Edit(int id)
        {
            var adminContract = context.AdminContracts.Find(id);
            if (adminContract == null)
            {
                errorViewModel = new ErrorViewModel { Message = "No Contract Valid With This ID", Controller = "AdminContract", Action = "Index" };
                return View("Error", errorViewModel);
            }
            return View(adminContract);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, AdminContract adminContract)
        {
            if (ModelState.IsValid)
            {
                context.AdminContracts.Update(adminContract);
                try
                {
                    context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    errorViewModel = new ErrorViewModel { Message = e.Message, Controller = "AdminContract", Action = "Index" };
                    return View("Error", errorViewModel);
                }
            }
            return View(adminContract);
        }

        public ActionResult Delete(int id)
        {
            var adminContract = context.AdminContracts.Find(id);
            if (adminContract == null)
            {
                errorViewModel = new ErrorViewModel { Message = "Contract Not Exist", Controller = "AdminContract", Action = "Index" };
                return View("Error", errorViewModel);
            }
            context.AdminContracts.Remove(adminContract);
            try
            {
                context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                errorViewModel = new ErrorViewModel { Message = e.Message, Controller = "AdminContract", Action = "Index" };
                return View("Error", errorViewModel);
            }
        }
    }
}
