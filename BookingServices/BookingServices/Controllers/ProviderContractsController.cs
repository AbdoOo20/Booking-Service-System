using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingServices.Data;
using BookingServices.Models;

namespace BookingServices.Controllers
{
    public class ProviderContractsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private ErrorViewModel errorViewModel;

        public ProviderContractsController(ApplicationDbContext context)
        {
            _context = context;
            errorViewModel = new ErrorViewModel { Message = "", Controller = "", Action = "" };
        }

        // GET: ProviderContracts
        public async Task<IActionResult> Index()
        {
            try
            {
                var applicationDbContext = _context.ProviderContracts.Include(p => p.ServiceProvider);
                return View(await applicationDbContext.ToListAsync());
            }
            catch (Exception e)
            {
                errorViewModel.Message = $"An error occurred: {e.Message}";
                errorViewModel.Controller = nameof(ProviderContractsController);
                errorViewModel.Action = nameof(Index);
                return View("Error", errorViewModel);
            }
        }

        // GET: ProviderContracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    errorViewModel.Message = "Contract ID cannot be null.";
                    errorViewModel.Controller = nameof(ProviderContractsController);
                    errorViewModel.Action = nameof(Details);
                    return View("Error", errorViewModel);
                }

                var providerContract = await _context.ProviderContracts
                    .Include(p => p.ServiceProvider)
                    .FirstOrDefaultAsync(m => m.ContractId == id);

                if (providerContract == null)
                {
                    errorViewModel.Message = $"Contract with ID {id} not found.";
                    errorViewModel.Controller = nameof(ProviderContractsController);
                    errorViewModel.Action = nameof(Details);
                    return View("Error", errorViewModel);
                }

                return View(providerContract);
            }
            catch (Exception e)
            {
                errorViewModel.Message = $"An error occurred: {e.Message}";
                errorViewModel.Controller = nameof(ProviderContractsController);
                errorViewModel.Action = nameof(Details);
                return View("Error", errorViewModel);
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
                    _context.Add(providerContract);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    errorViewModel.Message = $"An error occurred: {e.Message}";
                    errorViewModel.Controller = nameof(ProviderContractsController);
                    errorViewModel.Action = nameof(Create);
                    return View("Error", errorViewModel);
                }
            }
            return View(providerContract);
        }

        // GET: ProviderContracts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    errorViewModel.Message = "Contract ID cannot be null.";
                    errorViewModel.Controller = nameof(ProviderContractsController);
                    errorViewModel.Action = nameof(Edit);
                    return View("Error", errorViewModel);
                }

                var providerContract = await _context.ProviderContracts.FindAsync(id);
                if (providerContract == null)
                {
                    errorViewModel.Message = $"Contract with ID {id} not found.";
                    errorViewModel.Controller = nameof(ProviderContractsController);
                    errorViewModel.Action = nameof(Edit);
                    return View("Error", errorViewModel);
                }

                return View(providerContract);
            }
            catch (Exception e)
            {
                errorViewModel.Message = $"An error occurred: {e.Message}";
                errorViewModel.Controller = nameof(ProviderContractsController);
                errorViewModel.Action = nameof(Edit);
                return View("Error", errorViewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContractId,ContractName,Details")] ProviderContract providerContract)
        {
            if (id != providerContract.ContractId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(providerContract);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    errorViewModel.Message = $"An error occurred: {e.Message}";
                    errorViewModel.Controller = nameof(ProviderContractsController);
                    errorViewModel.Action = nameof(Edit);
                    return View("Error", errorViewModel);
                }
            }

            return View(providerContract);
        }

        // GET: ProviderContracts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    errorViewModel.Message = "Contract ID cannot be null.";
                    errorViewModel.Controller = nameof(ProviderContractsController);
                    errorViewModel.Action = nameof(Delete);
                    return View("Error", errorViewModel);
                }

                var providerContract = await _context.ProviderContracts
                    .Include(p => p.ServiceProvider)
                    .FirstOrDefaultAsync(m => m.ContractId == id);

                if (providerContract == null)
                {
                    errorViewModel.Message = $"Contract with ID {id} not found.";
                    errorViewModel.Controller = nameof(ProviderContractsController);
                    errorViewModel.Action = nameof(Delete);
                    return View("Error", errorViewModel);
                }

                return View(providerContract);
            }
            catch (Exception e)
            {
                errorViewModel.Message = $"An error occurred: {e.Message}";
                errorViewModel.Controller = nameof(ProviderContractsController);
                errorViewModel.Action = nameof(Delete);
                return View("Error", errorViewModel);
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
                    errorViewModel.Message = $"Contract with ID {id} not found.";
                    errorViewModel.Controller = nameof(ProviderContractsController);
                    errorViewModel.Action = nameof(DeleteConfirmed);
                    return View("Error", errorViewModel);
                }

                _context.ProviderContracts.Remove(providerContract);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                errorViewModel.Message = $"An error occurred: {e.Message}";
                errorViewModel.Controller = nameof(ProviderContractsController);
                errorViewModel.Action = nameof(DeleteConfirmed);
                return View("Error", errorViewModel);
            }
        }

        private bool ProviderContractExists(int id)
        {
            return _context.ProviderContracts.Any(e => e.ContractId == id);
        }
    }
}
