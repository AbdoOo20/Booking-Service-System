using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingServices.Data;
using BookingServices.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BookingServices.Controllers
{
    [Authorize(Roles = "Provider")]
    public class ProviderContractsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private ErrorViewModel errorViewModel;
        private readonly UserManager<IdentityUser> _userManager;
        string UserID;

        public ProviderContractsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            errorViewModel = new ErrorViewModel { Message = "", Controller = "", Action = "" };
            _userManager = userManager;
            UserID = "";
        }

        public async Task<string> GetCurrentUserID()
        {
            IdentityUser? user = await _userManager.GetUserAsync(User);
            return user.Id ?? "";
        }

        // Error handling function
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

        // GET: ProviderContracts
        public async Task<IActionResult> Index()
        {
            try
            {
                UserID = await GetCurrentUserID();
                var applicationDbContext = _context.ProviderContracts
                                                   .Where(p => p.ProviderId == UserID)
                                                   .Include(p => p.ServiceProvider);
                return View(await applicationDbContext.ToListAsync());
            }
            catch (Exception e)
            {
                return HandleError($"An error occurred while fetching provider contracts: {e.Message}", "ProviderHome", nameof(Index));
            }
        }

        // GET: ProviderContracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                // Bad request handled by HandleError function
                return HandleError("No contract ID was provided. Please try again.", "ProviderContracts", nameof(Index));
            }

            try
            {
                var providerContract = await _context.ProviderContracts
                    .Include(p => p.ServiceProvider)
                    .FirstOrDefaultAsync(m => m.ContractId == id);

                if (providerContract == null)
                {
                    // Not found handled by HandleError function
                    return HandleError($"We're sorry, but the contract could not be found. It may have been deleted or doesn't exist.", "ProviderContracts", nameof(Index));
                }

                return View(providerContract);
            }
            catch (Exception e)
            {
                return HandleError($"Something went wrong while processing your request. Technical details: {e.Message}", "ProviderContracts", nameof(Index));
            }
        }

        // GET: ProviderContracts/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContractId,ContractName,Details")] ProviderContract providerContract)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    UserID = await GetCurrentUserID();
                    providerContract.ProviderId = UserID;
                    _context.Add(providerContract);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    return HandleError($"An error occurred while creating the contract: {e.Message}", "ProviderContracts", nameof(Create));
                }
            }
            return View(providerContract);
        }

        // GET: ProviderContracts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return HandleError("Contract ID cannot be null.", "ProviderHome", nameof(Index));
            }

            try
            {
                var providerContract = await _context.ProviderContracts.FindAsync(id);
                if (providerContract == null)
                {
                    return HandleError($"Contract not found.", "ProviderHome", nameof(Index));
                }

                return View(providerContract);
            }
            catch (Exception e)
            {
                return HandleError($"An error occurred while fetching the contract details: {e.Message}", "ProviderHome", nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContractId,ContractName,Details")] ProviderContract providerContract)
        {
            if (id != providerContract.ContractId)
            {
                return HandleError("The contract ID does not match the provided contract data.", "ProviderHome", nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    UserID = await GetCurrentUserID();
                    providerContract.ProviderId = UserID;
                    _context.Update(providerContract);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    return HandleError($"An error occurred while updating the contract: {e.Message}", "ProviderHome", nameof(Index));
                }
            }

            return View(providerContract);
        }

        // GET: ProviderContracts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                // Bad request handled by HandleError function
                return HandleError("Contract ID cannot be null.", "ProviderContracts", nameof(Index));
            }

            try
            {
                var providerContract = await _context.ProviderContracts
                    .Include(p => p.ServiceProvider)
                    .FirstOrDefaultAsync(m => m.ContractId == id);

                if (providerContract == null)
                {
                    // Not found handled by HandleError function
                    return HandleError($"Contract with ID {id} not found.", "ProviderContracts", nameof(Index));
                }

                return View(providerContract);
            }
            catch (Exception e)
            {
                return HandleError($"An error occurred while fetching the contract: {e.Message}", "ProviderHome", nameof(Index));
            }
        }

        // POST: ProviderContracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var providerContract = await _context.ProviderContracts.FindAsync(id);

                if (providerContract == null)
                {
                    // Not found handled by HandleError function
                    return HandleError($"Contract with ID {id} not found.", "ProviderContracts", nameof(Index));
                }
                var services = _context.Services;
                foreach (var service in services)
                {
                    if (service.ProviderContractId == id)
                    {
                        errorViewModel = new ErrorViewModel
                        {
                            Message = "Cann't Block this Contract, becouse the are some services Using It, but you Can Edit the Contract Details",
                            Controller = "ProviderContracts",
                            Action = "Index"
                        };
                        return View("Error", errorViewModel);
                    }
                }
                providerContract.IsBlocked = !providerContract.IsBlocked;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                return HandleError($"An error occurred while updating the contract status: {e.Message}", "ProviderContracts", nameof(Index));
            }
        }
    }
}
