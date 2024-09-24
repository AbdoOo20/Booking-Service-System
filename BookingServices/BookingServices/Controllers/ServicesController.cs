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
        private readonly HttpClient _client;
        private readonly string SaudiArabiaRegionsCitiesAndDistricts;
        private readonly IWebHostEnvironment _environment;

        public ServicesController(ApplicationDbContext context, HttpClient client, IWebHostEnvironment environment)
        {
            _context = context;
            _client = client;
            SaudiArabiaRegionsCitiesAndDistricts = "https://raw.githubusercontent.com/homaily/Saudi-Arabia-Regions-Cities-and-Districts/refs/heads/master/json/regions_lite.json";
            _environment = environment;
        }

        // GET: Services
        public async Task<IActionResult> Index()
        {
            var services = _context.Services
                .Include(s => s.AdminContract)
                .Include(s => s.BaseService)
                .Include(s => s.Category)
                .Include(s => s.ProviderContract)
                .Include(s => s.ServiceProvider);

            return View(await services.ToListAsync());
        }

        /// <summary>
        /// Get Details of Each Service 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Services/Details/5
        // GET: Services/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Services.Where(m => m.ServiceId == id)
                .Select(s => new
                {
                    s.Name,
                    s.StartTime,
                    s.EndTime,
                    s.Quantity,
                    s.Location,
                    s.Details,
                    s.ServiceId
                })
                .FirstOrDefaultAsync();

            if (service == null)
            {
                return NotFound();
            }

            var CurrentDate = DateTime.Now.Date;

            var servicePrice = await (from s in _context.Services
                                      join sp in _context.ServicePrices
                                      on s.ServiceId equals sp.ServiceId
                                      where sp.PriceDate.Date == CurrentDate
                                      select new
                                      {
                                          ServicePrice = sp.Price
                                      }).FirstOrDefaultAsync();

            int daysBack = 1;
            while (servicePrice == null)
            {
                var previousDate = CurrentDate.AddDays(-daysBack);

                servicePrice = await (from s in _context.Services
                                      join sp in _context.ServicePrices
                                      on s.ServiceId equals sp.ServiceId
                                      where sp.PriceDate.Date == previousDate
                                      select new
                                      {
                                          ServicePrice = sp.Price
                                      }).FirstOrDefaultAsync();

                daysBack++;
            }

            var providerName = await (from s in _context.Services
                                      join sp in _context.ServiceProviders
                                      on s.ProviderId equals sp.ProviderId
                                      join a in _context.Users
                                      on sp.ProviderId equals a.Id
                                      select new
                                      {
                                          ProviderName = a.UserName
                                      }).FirstOrDefaultAsync();

            var providerID = await _context.Services.Where(s => s.ServiceId == id).Select(s => s.ProviderId).FirstOrDefaultAsync();

            var serviceImages = await (from s in _context.Services
                                       join si in _context.ServiceImages
                                       on s.ServiceId equals si.ServiceId
                                       where s.ServiceId == id
                                       select si.URL).ToListAsync();

            var categoryName = await (from s in _context.Services
                                      join c in _context.Categories
                                      on s.CategoryId equals c.CategoryId
                                      select new
                                      {
                                          CategoryName = c.Name
                                      }).FirstOrDefaultAsync();

            // Fix: Assign customerId inside the reviews query
            List<ReviewModel> reviews = await (from r in _context.Reviews
                                               join a in _context.Users on r.CustomerId equals a.Id
                                               select new ReviewModel
                                               {
                                                   ReviewerName = a.UserName,
                                                   BookingId = r.BookingId,
                                                   CustomerComment = r.CustomerComment,
                                                   CustomerCommentDate = r.CustomerCommentDate,
                                                   CustomerId = r.CustomerId,
                                                   ProviderReplayComment = r.ProviderReplayComment,
                                                   ProviderReplayCommentDate = r.ProviderReplayCommentDate
                                               }).ToListAsync();



            var numberOfReviews = await _context.Reviews
                   .CountAsync(r => r.CustomerComment != null);

            decimal averageRating = await _context.Reviews
                               .AverageAsync(r => r.Rating);

            ServiceDetailsModel serviceDetailsModel = new ServiceDetailsModel()
            {
                ServiceName = service.Name,
                servicePrice = servicePrice.ServicePrice,
                startTime = service.StartTime,
                endTime = service.EndTime,
                AvailableQuantity = service.Quantity,
                ServiceDetails = service.Details,
                serviceLocation = service.Location,
                providerName = providerName.ProviderName,
                CategoryName = categoryName.CategoryName,
                serviceImages = serviceImages,
                Reviews = reviews,
                providerRate = averageRating,
                numberOfReviews = numberOfReviews,
                ProviderID = providerID
            };

            return View(serviceDetailsModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> AddProviderReply(string customerId, string providerReply, int BookID)
        {
            if (string.IsNullOrWhiteSpace(customerId) || string.IsNullOrWhiteSpace(providerReply))
            {
                return Json(new { success = false, message = "Customer ID and provider reply cannot be empty." });
            }

            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.CustomerId == customerId && r.BookingId == BookID);

            if (review == null)
            {
                return Json(new { success = false, message = "Review not found." });
            }

            review.ProviderReplayComment = providerReply;
            review.ProviderReplayCommentDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                providerReply = review.ProviderReplayComment,
                providerReplyDate = review.ProviderReplayCommentDate
            });
        }
        // GET: Services/Create
        public async Task<IActionResult> CreateAsync()
        {
            await AddSelectLists();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Details,Location,StartTime,EndTime,Quantity,InitialPaymentPercentage,CategoryId,BaseServiceId,ProviderContractId,AdminContractId")] Service service, IFormFileCollection Images, decimal Price)
        {
            service.IsOnlineOrOffline = false;
            service.IsRequestedOrNot = false;

            if (ModelState.IsValid)
            {
                _context.Add(service);
                await _context.SaveChangesAsync();

                await FileUpload(Images, service.ServiceId);
                await ServicePrice(Price, service.ServiceId);

                return RedirectToAction(nameof(Index));
            }

            await AddSelectLists(service);
            return View(service);
        }

        // GET: Services/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var service = await _context.Services.FindAsync(id);
            if (service == null) return NotFound();

            await AddSelectLists(service);
            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ServiceId,Name,Details,Location,StartTime,EndTime,Quantity,InitialPaymentPercentage,CategoryId,BaseServiceId,ProviderContractId,AdminContractId,ProviderId")] Service service, IFormFileCollection Images, decimal Price)
        {
            if (id != service.ServiceId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(service);
                    await _context.SaveChangesAsync();

                    await FileUpload(Images, service.ServiceId);
                    await ServicePrice(Price, service.ServiceId);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceExists(service.ServiceId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            await AddSelectLists(service);
            return View(service);
        }

        // GET: Services/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var service = await _context.Services
                .Include(s => s.AdminContract)
                .Include(s => s.BaseService)
                .Include(s => s.Category)
                .Include(s => s.ProviderContract)
                .Include(s => s.ServiceProvider)
                .FirstOrDefaultAsync(m => m.ServiceId == id);

            if (service == null) return NotFound();

            return View(service);
        }

        // POST: Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null) return NotFound();

            var hasBookings = _context.BookingServices.Any(b => b.ServiceId == id);
            if (hasBookings)
            {
                ViewData["ServiceBooking"] = _context.BookingServices.FirstOrDefault(b => b.ServiceId == id);
                return View(service);
            }

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Services/Images/5
        public async Task<IActionResult> Images(int id)
        {
            if (!ServiceExists(id)) return NotFound();

            var serviceImages = _context.ServiceImages.Where(s => s.ServiceId == id).Include(s => s.Service);
            var service = await _context.Services.FindAsync(id);
            if (service == null) return NotFound();
            ViewData["ServiceName"] = service.Name;
            ViewData["ServiceId"] = service.ServiceId;

            return View(await serviceImages.ToListAsync());
        }

        // GET: AddImage
        public Task<IActionResult> AddImage(int id)
        {
            if (!ServiceExists(id))
                return Task.FromResult<IActionResult>(NotFound());

            ViewData["ServiceId"] = id;
            return Task.FromResult<IActionResult>(View());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddImage(int id, IFormFileCollection Images)
        {
            if (!ServiceExists(id)) return NotFound();

            await FileUpload(Images, id);
            return RedirectToAction(nameof(Images), new { id });
        }

        public async Task<IActionResult> DeleteImage(int id, [FromQuery] string url)
        {
            if (!ServiceExists(id)) return NotFound();

            var serviceImage = await _context.ServiceImages
                .FirstOrDefaultAsync(si => si.ServiceId == id && si.URL == url);

            if (serviceImage == null) return NotFound();

            _context.ServiceImages.Remove(serviceImage);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Images), new { id });
        }

        // GET: Prices/5
        public async Task<IActionResult> Prices(int id)
        {
            if (!ServiceExists(id)) return NotFound();

            var servicePrices = _context.ServicePrices.Where(s => s.ServiceId == id).Include(s => s.Service);
            var service = await _context.Services.FindAsync(id);
            if (service == null) return NotFound();
            ViewData["ServiceName"] = service.Name;
            ViewData["ServiceId"] = service.ServiceId;

            return View(await servicePrices.ToListAsync());
        }

        // GET: AddPrice
        public IActionResult AddPrice(int id)
        {
            ViewData["ServiceId"] = id;
            return View();
        }

        // POST: AddPrice
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPrice(int id, [Bind("ServiceId,PriceDate,Price")] ServicePrice servicePrice)
        {
            if (ModelState.IsValid)
            {
                servicePrice.ServiceId = id;
                _context.ServicePrices.Add(servicePrice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Prices), new { id = id });
            }
            return View(servicePrice);
        }

        public async Task<IActionResult> RequestToMakeServiceOnline(int id)
        {
            if (ServiceExists(id))
            {
                var service = await _context.Services.FindAsync(id);
                if (service != null)
                {
                    service.IsRequestedOrNot = true;
                    _context.Entry(service).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // Helper Method: Check if a service exists
        private bool ServiceExists(int id)
        {
            return _context.Services.Any(e => e.ServiceId == id);
        }

        // Helper Method: Populate SelectLists
        private async Task AddSelectLists(Service? service = null)
        {
            var response = await _client.GetAsync(SaudiArabiaRegionsCitiesAndDistricts);
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var Regions = JsonConvert.DeserializeObject<List<Region>>(jsonData);
                ViewData["Location"] = new SelectList(Regions, "name_en", "name_en");
            }
            if (service != null) ViewData["BaseServiceId"] = new SelectList(_context.Services.Where(s => s.ServiceId != service.ServiceId), "ServiceId", "Name", service?.BaseServiceId);
            else ViewData["BaseServiceId"] = new SelectList(_context.Services, "ServiceId", "Name", service?.BaseServiceId);
            ViewData["AdminContractId"] = new SelectList(_context.AdminContracts, "ContractId", "ContractName", service?.AdminContractId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", service?.CategoryId);
            ViewData["ProviderContractId"] = new SelectList(_context.ProviderContracts, "ContractId", "ContractName", service?.ProviderContractId);
        }

        // Helper Method: Handle File Uploads
        private async Task FileUpload(IFormFileCollection Images, int serviceId)
        {
            if (Images != null && Images.Any())
            {
                var uploadPath = Path.Combine(_environment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadPath); // Ensure directory exists

                foreach (var image in Images)
                {
                    if (image.Length > 0)
                    {
                        var filePath = Path.Combine(uploadPath, image.FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        var serviceImage = new ServiceImage
                        {
                            ServiceId = serviceId,
                            URL = $"/uploads/{image.FileName}"
                        };

                        _context.ServiceImages.Add(serviceImage);
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }

        // Helper Method: Handle Service Price
        private async Task ServicePrice(decimal price, int serviceId)
        {
            if (price > 0)
            {
                var servicePrice = new ServicePrice
                {
                    ServiceId = serviceId,
                    PriceDate = DateTime.Now,
                    Price = price
                };
                _context.ServicePrices.Add(servicePrice);
                await _context.SaveChangesAsync();
            }
        }
    }
}
