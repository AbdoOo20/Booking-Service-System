using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookingServices.Data;
using Newtonsoft.Json;
using BookingServices.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using BookingServices.ViewModel;

namespace BookingServices.Controllers
{
    [Authorize("PROVIDER")]
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _client;
        private readonly string SaudiArabiaRegionsCitiesAndDistricts;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<IdentityUser> _userManager;
        string UserID;
        ErrorViewModel errorViewModel;

        public ServicesController(ApplicationDbContext context, HttpClient client, IWebHostEnvironment environment, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _client = client;
            SaudiArabiaRegionsCitiesAndDistricts = "https://raw.githubusercontent.com/homaily/Saudi-Arabia-Regions-Cities-and-Districts/refs/heads/master/json/regions_lite.json";
            _environment = environment;
            _userManager = userManager;
            UserID = "";
            errorViewModel = new ErrorViewModel();
        }

        public async Task<string> GetCurrentUserID()
        {
            IdentityUser? user = await _userManager.GetUserAsync(User);
            return user.Id ?? "";
        }
        public IActionResult ErrorHandling(string action, string message)
        {
            errorViewModel.Controller = "Services";
            errorViewModel.Action = action;
            errorViewModel.Message = message;

            return View("Error", errorViewModel);
        }


        // GET: Services
        public async Task<IActionResult> Index()
        {
            IQueryable<Service> services = Enumerable.Empty<Service>().AsQueryable();
            try
            {
                UserID = await GetCurrentUserID();
                services = _context.Services.Where(s => s.ProviderId == UserID);
            }
            catch (Exception e)
            {
                ErrorHandling(nameof(Index),e.Message);
            }

            return View(services.ToList());
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
                ErrorHandling(nameof(Details), "The ID is Required");
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
                                          ProviderName = sp.Name
                                      }).FirstOrDefaultAsync();

            var providerID = await _context.Services.Where(s => s.ServiceId == id).Select(s => s.ProviderId).FirstOrDefaultAsync();

            var serviceImages = await (from s in _context.Services
                                       join si in _context.ServiceImages
                                       on s.ServiceId equals si.ServiceId
                                       where s.ServiceId == id
            select si.URL).ToListAsync();

            var CategoryName = await(from s in _context.Services
                                      join c in _context.Categories on s.CategoryId equals c.CategoryId
                                      where s.ServiceId == id
                                      select c.Name).FirstOrDefaultAsync();

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



            var numberOfReviews = await (from r in _context.Reviews
                                         join bs in _context.BookingServices
                                         on r.BookingId equals bs.BookingId
                                         where bs.ServiceId == id
                                         select r.CustomerId).CountAsync();
            decimal averageRating = 0;
            if (numberOfReviews > 0)
            {
                averageRating = await (from r in _context.Reviews
                                       join bs in _context.BookingServices
                                       on r.BookingId equals bs.BookingId
                                       where bs.ServiceId == id
                                       select r.Rating).AverageAsync();
            }
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
                CategoryName = CategoryName,
                serviceImages = serviceImages,
                Reviews = reviews,
                providerRate = Math.Round(averageRating, 1),
                numberOfReviews = numberOfReviews,
                ProviderID = providerID,
                ServiceId = id
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
            try
            {
                UserID = await GetCurrentUserID();
            }
            catch (Exception e)
            {
                ErrorHandling(nameof(Create),e.Message);
            }
            service.ProviderId = UserID;
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

            if (id == null)
                ErrorHandling(nameof(Edit), "ID is Required !!!");

            var service = await _context.Services.FindAsync(id);
            if (service == null)
                ErrorHandling(nameof(Edit), "The Service Dose Not Exists !!!");

            await AddSelectLists(service);
            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ServiceId,Name,Details,Location,StartTime,EndTime,Quantity,InitialPaymentPercentage,CategoryId,BaseServiceId,ProviderContractId,AdminContractId,ProviderId")] Service service, IFormFileCollection Images, decimal Price)
        {
            if (id != service.ServiceId) return NotFound();
            try
            {
                UserID = await GetCurrentUserID();
            }
            catch (Exception e)
            {
                ErrorHandling(nameof(Edit), e.Message);
            }
            service.ProviderId = UserID;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(service);
                    await _context.SaveChangesAsync();

                    await FileUpload(Images, service.ServiceId);
                    await ServicePrice(Price, service.ServiceId);
                }
                catch (Exception e)
                {
                    ErrorHandling(nameof(Edit), e.Message);
                }
                return RedirectToAction(nameof(Index));
            }

            await AddSelectLists(service);
            return View(service);
        }

        // GET: Services/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                ErrorHandling(nameof(Edit), "ID is Required !!!");

            var service = await _context.Services.FindAsync(id);
            if (service == null)
                ErrorHandling(nameof(Edit), "The Service Dose Not Exists !!!");

            return View(service);
        }

        // POST: Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
                ErrorHandling(nameof(Delete), "The Service Dose Not Exists !!!");


            var hasBookings = _context.BookingServices.Any(b => b.ServiceId == id);
            if (hasBookings)
            {
                ViewData["ServiceBooking"] = _context.BookingServices.FirstOrDefault(b => b.ServiceId == id);
                return View(service);
            }

            try
            {
                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ErrorHandling(nameof(Delete), ex.Message);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Services/Images/5
        public async Task<IActionResult> Images(int id)
        {
            if (!ServiceExists(id))
                ErrorHandling(nameof(Images), "The Service Dose Not Exists !!!");

            var serviceImages = _context.ServiceImages.Where(s => s.ServiceId == id).Include(s => s.Service);
            var service = await _context.Services.FindAsync(id);
            ViewData["ServiceName"] = service.Name;
            ViewData["ServiceId"] = service.ServiceId;

            return View(await serviceImages.ToListAsync());
        }

        // GET: AddImage
        public Task<IActionResult> AddImage(int id)
        {
            if (!ServiceExists(id))
                ErrorHandling(nameof(AddImage), "The Service Dose Not Exists !!!");

            ViewData["ServiceId"] = id;
            return Task.FromResult<IActionResult>(View());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddImage(int id, IFormFileCollection Images)
        {
            if (!ServiceExists(id))
                ErrorHandling(nameof(AddImage), "The Service Dose Not Exists !!!");

            await FileUpload(Images, id);
            return RedirectToAction(nameof(Images), new { id });
        }

        public async Task<IActionResult> DeleteImage(int id, [FromQuery] string url)
        {
            if (!ServiceExists(id))
                ErrorHandling(nameof(DeleteImage), "The Service Dose Not Exists !!!");

            var serviceImage = await _context.ServiceImages
                .FirstOrDefaultAsync(si => si.ServiceId == id && si.URL == url);

            if (serviceImage == null) 
                ErrorHandling(nameof(DeleteImage), "The Service Dose Not Exists !!!");


            _context.ServiceImages.Remove(serviceImage);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Images), new { id });
        }

        // GET: Prices/5
        public async Task<IActionResult> Prices(int id)
        {
            if (!ServiceExists(id)) 
                ErrorHandling(nameof(Prices), "The Service Dose Not Exists !!!");

            var servicePrices = _context.ServicePrices.Where(s => s.ServiceId == id).Include(s => s.Service);
            var service = await _context.Services.FindAsync(id);
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
