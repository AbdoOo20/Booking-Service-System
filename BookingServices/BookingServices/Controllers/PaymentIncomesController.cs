using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BookingServices.Data;
using BookingServices.Models;

namespace BookingServices.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PaymentIncomesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentIncomesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PaymentIncomes
        public async Task<IActionResult> Index()
        {
            try
            {
                var paymentIncomes = await _context.PaymentIncomes
                    .Select(pi => new PaymentMethodDTO
                    {
                        PaymentIncomeId = pi.PaymentIncomeId,
                        Name = pi.Name,
                        Percentage = pi.Percentage,
                        IsBlocked = pi.IsBlocked
                    })
                    .ToListAsync();

                return View(paymentIncomes);
            }
            catch (Exception)
            {
                ViewData["ErrorMessage"] = "An error occurred while loading the data. Please try again.";
                return View(Enumerable.Empty<PaymentMethodDTO>());
            }
        }

        // GET: PaymentIncomes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var paymentIncome = await _context.PaymentIncomes
                    .FirstOrDefaultAsync(m => m.PaymentIncomeId == id);

                if (paymentIncome == null) return NotFound();

                return View(ToDTO(paymentIncome));
            }
            catch (Exception)
            {
                ViewData["ErrorMessage"] = "An error occurred while loading the details. Please try again.";
                return View();
            }
        }

        // GET: PaymentIncomes/Create
        public IActionResult Create()
        {
            return View(new PaymentMethodDTO());
        }

        // POST: PaymentIncomes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Percentage")] PaymentMethodDTO dto)
        {
            if (!ModelState.IsValid) return View(dto);

            try
            {
                var paymentIncome = FromDTO(dto);
                paymentIncome.IsBlocked = false;

                _context.Add(paymentIncome);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ViewData["ErrorMessage"] = "An error occurred while saving the data. Please try again later.";
                return View(dto);
            }
            catch (Exception)
            {
                ViewData["ErrorMessage"] = "An unexpected error occurred. Please try again.";
                return View(dto);
            }
        }

        // GET: PaymentIncomes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var paymentIncome = await _context.PaymentIncomes.FindAsync(id);
                if (paymentIncome == null) return NotFound();

                return View(ToDTO(paymentIncome));
            }
            catch (Exception)
            {
                ViewData["ErrorMessage"] = "An error occurred while loading the data. Please try again.";
                return View();
            }
        }

        // POST: PaymentIncomes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PaymentIncomeId,Name,Percentage")] PaymentMethodDTO dto)
        {
            if (id != dto.PaymentIncomeId) return NotFound();
            if (!ModelState.IsValid) return View(dto);

            try
            {
                var paymentIncome = await _context.PaymentIncomes.FindAsync(id);
                if (paymentIncome == null) return NotFound();

                paymentIncome.Name = dto.Name;
                paymentIncome.Percentage = dto.Percentage;

                _context.Update(paymentIncome);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentIncomeExists(dto.PaymentIncomeId))
                    return NotFound();
                throw;
            }
            catch (DbUpdateException)
            {
                ViewData["ErrorMessage"] = "An error occurred while saving the changes. Please try again.";
                return View(dto);
            }
            catch (Exception)
            {
                ViewData["ErrorMessage"] = "An unexpected error occurred. Please try again.";
                return View(dto);
            }
        }

        // POST: PaymentIncomes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var paymentIncome = await _context.PaymentIncomes.FindAsync(id);
                if (paymentIncome == null) return NotFound();

                _context.PaymentIncomes.Remove(paymentIncome);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ViewData["ErrorMessage"] = "An error occurred while deleting the item. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleBlock(int id)
        {
            try
            {
                var gateway = await _context.PaymentIncomes.FindAsync(id);
                if (gateway == null)
                    return Json(new { success = false, message = "Payment Method Not Found" });

                gateway.IsBlocked = !(gateway.IsBlocked ?? false);
                await _context.SaveChangesAsync();

                return Json(new { success = true, isBlocked = gateway.IsBlocked });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred. Please try again." });
            }
        }

        private bool PaymentIncomeExists(int id)
        {
            return _context.PaymentIncomes.Any(e => e.PaymentIncomeId == id);
        }

        private PaymentMethodDTO ToDTO(PaymentIncome income)
        {
            return new PaymentMethodDTO
            {
                PaymentIncomeId = income.PaymentIncomeId,
                Name = income.Name,
                Percentage = income.Percentage,
                IsBlocked = income.IsBlocked
            };
        }

        private PaymentIncome FromDTO(PaymentMethodDTO dto)
        {
            return new PaymentIncome
            {
                PaymentIncomeId = dto.PaymentIncomeId,
                Name = dto.Name,
                Percentage = dto.Percentage,
                IsBlocked = dto.IsBlocked
            };
        }
    }
}
