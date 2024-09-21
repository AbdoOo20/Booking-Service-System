using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookingServices.Data;
using Newtonsoft.Json;
using BookingServices.Models;

namespace BookingServices.Controllers
{
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _context;
        HttpClient _client;
        private readonly string SaudiArabiaRegionsCitiesAndDistricts;

        public ServicesController(ApplicationDbContext context, HttpClient client)
        {
            _context = context;
            _client = client;
            SaudiArabiaRegionsCitiesAndDistricts = "https://raw.githubusercontent.com/homaily/Saudi-Arabia-Regions-Cities-and-Districts/refs/heads/master/json/regions_lite.json";

        }

        // GET: Services
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Services.Include(s => s.AdminContract).Include(s => s.BaseService).Include(s => s.Category).Include(s => s.ProviderContract).Include(s => s.ServiceProvider);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Services/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Services
                .Include(s => s.AdminContract)
                .Include(s => s.BaseService)
                .Include(s => s.Category)
                .Include(s => s.ProviderContract)
                .Include(s => s.ServiceProvider)
                .FirstOrDefaultAsync(m => m.ServiceId == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        // GET: Services/Create
        public async Task<IActionResult> CreateAsync()
        {
            var response = await _client.GetAsync(SaudiArabiaRegionsCitiesAndDistricts);

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var Regions = JsonConvert.DeserializeObject<List<Region>>(jsonData);
                ViewData["Location"] = new SelectList(Regions, "name_en", "name_en");
            }
            ViewData["AdminContractId"] = new SelectList(_context.AdminContracts, "ContractId", "Details");
            ViewData["BaseServiceId"] = new SelectList(_context.Services, "ServiceId", "Name");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name");
            ViewData["ProviderContractId"] = new SelectList(_context.ProviderContracts, "ContractId", "Details");
            ViewData["ProviderId"] = new SelectList(_context.ServiceProviders, "ProviderId", "ProviderId");
            return View();
        }

        // POST: Services/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ServiceId,Name,Details,Location,StartTime,EndTime,Quantity,InitialPaymentPercentage,IsOnlineOrOffline,IsRequestedOrNot,CategoryId,BaseServiceId,ProviderContractId,AdminContractId,ProviderId")] Service service)
        {
            if (ModelState.IsValid)
            {
                _context.Add(service);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AdminContractId"] = new SelectList(_context.AdminContracts, "ContractId", "Details", service.AdminContractId);
            ViewData["BaseServiceId"] = new SelectList(_context.Services, "ServiceId", "Details", service.BaseServiceId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", service.CategoryId);
            ViewData["ProviderContractId"] = new SelectList(_context.ProviderContracts, "ContractId", "Details", service.ProviderContractId);
            ViewData["ProviderId"] = new SelectList(_context.ServiceProviders, "ProviderId", "ProviderId", service.ProviderId);
            return View(service);
        }

        // GET: Services/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }
            ViewData["AdminContractId"] = new SelectList(_context.AdminContracts, "ContractId", "Details", service.AdminContractId);
            ViewData["BaseServiceId"] = new SelectList(_context.Services, "ServiceId", "Details", service.BaseServiceId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", service.CategoryId);
            ViewData["ProviderContractId"] = new SelectList(_context.ProviderContracts, "ContractId", "Details", service.ProviderContractId);
            ViewData["ProviderId"] = new SelectList(_context.ServiceProviders, "ProviderId", "ProviderId", service.ProviderId);
            return View(service);
        }

        // POST: Services/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ServiceId,Name,Details,Location,StartTime,EndTime,Quantity,InitialPaymentPercentage,IsOnlineOrOffline,IsRequestedOrNot,CategoryId,BaseServiceId,ProviderContractId,AdminContractId,ProviderId")] Service service)
        {
            if (id != service.ServiceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(service);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceExists(service.ServiceId))
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
            ViewData["AdminContractId"] = new SelectList(_context.AdminContracts, "ContractId", "Details", service.AdminContractId);
            ViewData["BaseServiceId"] = new SelectList(_context.Services, "ServiceId", "Details", service.BaseServiceId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", service.CategoryId);
            ViewData["ProviderContractId"] = new SelectList(_context.ProviderContracts, "ContractId", "Details", service.ProviderContractId);
            ViewData["ProviderId"] = new SelectList(_context.ServiceProviders, "ProviderId", "ProviderId", service.ProviderId);
            return View(service);
        }

        // GET: Services/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Services
                .Include(s => s.AdminContract)
                .Include(s => s.BaseService)
                .Include(s => s.Category)
                .Include(s => s.ProviderContract)
                .Include(s => s.ServiceProvider)
                .FirstOrDefaultAsync(m => m.ServiceId == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        // POST: Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServiceExists(int id)
        {
            return _context.Services.Any(e => e.ServiceId == id);
        }
    }
}
