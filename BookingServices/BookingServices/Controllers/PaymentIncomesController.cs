using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookingServices.Data;
using Microsoft.AspNetCore.Authorization;

namespace BookingServices.Controllers
{
    [Authorize("ADMIN")]
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
            return View(await _context.PaymentIncomes.ToListAsync());
        }

        // GET: PaymentIncomes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentIncome = await _context.PaymentIncomes
                .FirstOrDefaultAsync(m => m.PaymentIncomeId == id);
            if (paymentIncome == null)
            {
                return NotFound();
            }

            return View(paymentIncome);
        }

        // GET: PaymentIncomes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PaymentIncomes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentIncomeId,Name,Percentage")] PaymentIncome paymentIncome)
        {
            if (ModelState.IsValid)
            {
                _context.Add(paymentIncome);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(paymentIncome);
        }

        // GET: PaymentIncomes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentIncome = await _context.PaymentIncomes.FindAsync(id);
            if (paymentIncome == null)
            {
                return NotFound();
            }
            return View(paymentIncome);
        }

        // POST: PaymentIncomes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PaymentIncomeId,Name,Percentage")] PaymentIncome paymentIncome)
        {
            if (id != paymentIncome.PaymentIncomeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paymentIncome);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentIncomeExists(paymentIncome.PaymentIncomeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(paymentIncome);
        }

        // GET: PaymentIncomes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentIncome = await _context.PaymentIncomes
                .FirstOrDefaultAsync(m => m.PaymentIncomeId == id);
            if (paymentIncome == null)
            {
                return NotFound();
            }

            return View(paymentIncome);
        }

        // POST: PaymentIncomes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paymentIncome = await _context.PaymentIncomes.FindAsync(id);
            if (paymentIncome != null)
            {
                _context.PaymentIncomes.Remove(paymentIncome);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentIncomeExists(int id)
        {
            return _context.PaymentIncomes.Any(e => e.PaymentIncomeId == id);
        }
    }
}
