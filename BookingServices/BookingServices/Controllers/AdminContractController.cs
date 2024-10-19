using BookingServices.Data;
using BookingServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;

namespace BookingServices.Controllers
{
    [Authorize(Roles = "Admin ,Provider")]
    public class AdminContractController : Controller
    {
        ApplicationDbContext context;
        ErrorViewModel errorViewModel = new ErrorViewModel { Message = "", Controller = "", Action = "" };

        public AdminContractController([FromServices] ApplicationDbContext _context)
        {
            context = _context;
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(context.AdminContracts.ToList());
        }

        private IActionResult HandleError(string message, string controller, string action)
        {
            var errorViewModel = new ErrorViewModel
            {
                Message = message,
                Controller = controller,
                Action = action
            };
            return View("Error", errorViewModel);
        }


        // Action to return the Admin Contract details view
        [HttpGet]
        [Authorize(Roles = "Admin ,Provider")]
        public async Task<IActionResult> GetAdminContractDetails(int id)
        {
            var contract = await context.AdminContracts.FindAsync(id);

            if (contract == null)
            {
                return NotFound();
            }

            return PartialView("_AdminContractDetailsPartial", contract);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                // Bad request handled by HandleError function
                return HandleError("Contract ID cannot be null.", "AdminContract", nameof(Index));
            }

            try
            {
                var adminContract = await context.AdminContracts.FindAsync(id);

                if (adminContract == null)
                {
                    return HandleError($"Contract with ID {id} not found.", "AdminContract", nameof(Index));
                }

                return View(adminContract);
            }
            catch (Exception e)
            {
                return HandleError($"An error occurred while fetching the contract: {e.Message}", "AdminContract", nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var adminContract = context.AdminContracts.Find(id);
            if (adminContract == null)
            {
                errorViewModel = new ErrorViewModel { Message = "Contract Not Exist", Controller = "AdminContract", Action = "Index" };
                return View("Error", errorViewModel);
            }
            var services = context.Services;
            foreach (var service in services)
            {
                if (service.AdminContractId == id) 
                {
                    errorViewModel = new ErrorViewModel { Message = "Cann't Block this Contract, becouse the are some services Using It, but you Can Edit the Contract Details",
                         Controller = "AdminContract", Action = "Index" };
                    return View("Error", errorViewModel);
                }
            }
            adminContract.IsBlocked = adminContract.IsBlocked == null ? false : adminContract.IsBlocked;
            adminContract.IsBlocked = !adminContract.IsBlocked;
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
