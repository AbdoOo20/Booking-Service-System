using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookingServices.Data;
using Newtonsoft.Json;
using BookingServices.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BookingServices.Controllers
{
    [Authorize(Roles = "Admin,Provider")]
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

        // GET: Services
        public async Task<IActionResult> Index()
        {
            try
            {
                List<ServiceModel> servicesIndexModel = new List<ServiceModel>();
                // Fetch User ID
                UserID = await GetCurrentUserID();
                IdentityUser? user = await _userManager.FindByIdAsync(UserID);

                // If the user is not found, check if the UserID is in the Provider role
                bool isProvider = user != null && await _userManager.IsInRoleAsync(user, "Provider");
                ViewBag.isProvider = isProvider;

                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    servicesIndexModel = await _context.Services
                    .Include(s => s.Category)
                    .Include(s => s.ServiceProvider)
                    .Select(service => new ServiceModel
                    {
                        ServiceId = service.ServiceId,
                        Name = service.Name,
                        Location = service.Location ?? "Not Exists",
                        StartTime = service.StartTime.Hours,
                        EndTime = service.EndTime.Minutes > 0 ? 24 : service.EndTime.Hours,
                        Quantity = service.Quantity,
                        InitialPaymentPercentage = service.InitialPaymentPercentage,
                        IsOnlineOrOffline = service.IsOnlineOrOffline,
                        IsRequestedOrNot = service.IsRequestedOrNot,
                        CategoryName = service.Category.Name ?? "Not Exists",
                        ServiceProviderName = service.ServiceProvider.Name ?? "Not Exists"
                    })
                    .ToListAsync();
                }
                else
                { // Fetch services for the current provider
                    servicesIndexModel = await _context.Services
                        .Where(s => s.ProviderId == UserID)
                        .Include(s => s.Category)
                        .Select(service => new ServiceModel
                        {
                            ServiceId = service.ServiceId,
                            Name = service.Name,
                            Location = service.Location ?? "Not Exists",
                            StartTime = service.StartTime.Hours,
                            EndTime = service.EndTime.Minutes > 0 ? 24 : service.EndTime.Hours,
                            Quantity = service.Quantity,
                            InitialPaymentPercentage = service.InitialPaymentPercentage,
                            IsOnlineOrOffline = service.IsOnlineOrOffline,
                            IsRequestedOrNot = service.IsRequestedOrNot,
                            CategoryName = service.Category.Name ?? "Not Exists"
                        })
                        .ToListAsync();
                }

                foreach (var service in servicesIndexModel)
                {
                    service.ServicePrice = await _context.ServicePrices
                        .Where(s => s.ServiceId == service.ServiceId
                        && s.PriceDate.Date == DateTime.Now.Date)
                        .Select(s => s.Price)
                        .FirstOrDefaultAsync();
                }

                // Fetch Regions data from external API for locations
                var response = await _client.GetAsync(SaudiArabiaRegionsCitiesAndDistricts);
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var regions = JsonConvert.DeserializeObject<List<Region>>(jsonData);
                    ViewBag.Locations = regions.Select(r => r.name_en.Trim()).Distinct().ToList(); // Ensure no duplicates
                }

                // Get all category names and pass to ViewBag
                var categories = await _context.Categories
                    .Select(c => c.Name.Trim())
                    .Distinct()
                    .ToListAsync();

                ViewBag.Categories = categories;

                return View(servicesIndexModel);
            }
            catch (Exception e)
            {
                return HandleError($"Error: {e.Message}, StackTrace: {e.StackTrace}", "ProviderHome", nameof(Index));
            }
        }


        /// <summary>
        /// Get Details of Each Service 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Services/Details/5
        [Authorize("Provider")]
        [Authorize("Provider")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return HandleError("The ID is Required", "Services", nameof(Index));
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
                return HandleError("Service Not Found", "Services", nameof(Index));
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

            /*  var providerName = await (from s in _context.Services
                                        join sp in _context.ServiceProviders
                                        on s.ProviderId equals sp.ProviderId
                                        join a in _context.Users
                                        on sp.ProviderId equals a.Id
                                        select new
                                        {
                                            ProviderName = sp.Name
                                        }).FirstOrDefaultAsync();*/
            var providerName = await _context.Services.Where(s => s.ServiceId == id).Select(s => s.ServiceProvider.Name).FirstOrDefaultAsync();

            var providerID = await _context.Services.Where(s => s.ServiceId == id).Select(s => s.ProviderId).FirstOrDefaultAsync();

            var serviceImages = await (from s in _context.Services
                                       join si in _context.ServiceImages
                                       on s.ServiceId equals si.ServiceId
                                       where s.ServiceId == id
                                       select si.URL).ToListAsync();

            /*   var CategoryName = await(from s in _context.Services
                                         join c in _context.Categories on s.CategoryId equals c.CategoryId
                                         where s.ServiceId == id
                                         select c.Name).FirstOrDefaultAsync();*/
            var CategoryName = await _context.Services.Where(s => s.ServiceId == id).Select(s => s.Category.Name).FirstOrDefaultAsync();

            var bookingServicesIds = _context.BookingServices
                .Where(bs => bs.ServiceId == id)
                .Select(bs => bs.BookingId);

            List<Review> _reviews = new List<Review>();
            decimal sumationOfRatings = 0.0m;
            int numberOfReviews = 0;
            decimal averageRating = 0;

            foreach (var bookId in bookingServicesIds)
            {
                Review? review = await _context.Reviews
                    .Where(r => r.BookingId == bookId)
                    .Where((r => r.CustomerComment != null))
                    .FirstOrDefaultAsync();
                if (review != null)
                {
                    _reviews.Add(review);
                    sumationOfRatings += review.Rating;
                    numberOfReviews++;
                }
            }


            averageRating = numberOfReviews == 0 ? 0 : (sumationOfRatings / numberOfReviews);

            List<ReviewModel> reviews = new List<ReviewModel>();
            foreach (var review in _reviews)
            {
                reviews.Add(new ReviewModel
                {
                    BookingId = review.BookingId,
                    CustomerComment = review.CustomerComment,
                    CustomerId = review.CustomerId,
                    CustomerCommentDate = review.CustomerCommentDate,
                    ProviderReplayComment = review.ProviderReplayComment,
                    ProviderReplayCommentDate = review.ProviderReplayCommentDate,
                    ReviewerName = await _context.Customers.Where(c => c.CustomerId == review.CustomerId).Select(c => c.Name).FirstOrDefaultAsync()
                });
            }
            //// Fix: Assign customerId inside the reviews query
            //List<ReviewModel> reviews = await (from r in _context.Reviews
            //                                   join a in _context.Users 
            //                                   on r.CustomerId equals a.Id
            //                                   select new ReviewModel
            //                                   {
            //                                       ReviewerName = a.UserName,
            //                                       BookingId = r.BookingId,
            //                                       CustomerComment = r.CustomerComment,
            //                                       CustomerCommentDate = r.CustomerCommentDate,
            //                                       CustomerId = r.CustomerId,
            //                                       ProviderReplayComment = r.ProviderReplayComment,
            //                                       ProviderReplayCommentDate = r.ProviderReplayCommentDate
            //                                   }).ToListAsync();



            //var numberOfReviews = await (from r in _context.Reviews
            //                             join bs in _context.BookingServices
            //                             on r.BookingId equals bs.BookingId
            //                             where bs.ServiceId == id
            //                             select r.CustomerId).CountAsync();
            //decimal averageRating = 0;
            //if (numberOfReviews > 0)
            //{
            //    averageRating = await (from r in _context.Reviews
            //                           join bs in _context.BookingServices
            //                           on r.BookingId equals bs.BookingId
            //                           where bs.ServiceId == id
            //                           select r.Rating).AverageAsync();
            //}
            ServiceDetailsModel serviceDetailsModel = new ServiceDetailsModel()
            {
                ServiceName = service.Name,
                servicePrice = servicePrice.ServicePrice,
                startTime = service.StartTime,
                endTime = service.EndTime,
                AvailableQuantity = service.Quantity,
                ServiceDetails = service.Details,
                serviceLocation = service.Location,
                providerName = providerName,
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
        [Authorize("Provider")]
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

        [HttpGet]
        [Authorize("Provider")]
        public async Task<IActionResult> Create()
        {
            await AddSelectLists();
            return View();
        }

        // POST: Services/Create
        [HttpPost]
        [Authorize("Provider")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceModel service, IFormFileCollection Images)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    UserID = await GetCurrentUserID();
                    // Create a new Service entity from the submitted form
                    var newService = new Service
                    {
                        Name = service.Name,
                        Details = service.Details,
                        Location = service.Location,
                        StartTime = new TimeSpan(service.StartTime, 0, 0),
                        EndTime = service.EndTime == 24
                        ? new TimeSpan(service.StartTime - 1, 59, 59)
                        : new TimeSpan(service.StartTime, 0, 0),
                        Quantity = service.Quantity ?? 0,
                        InitialPaymentPercentage = service.InitialPaymentPercentage,
                        IsOnlineOrOffline = service.IsOnlineOrOffline,
                        IsRequestedOrNot = service.IsRequestedOrNot,
                        CategoryId = service.CategoryId,
                        AdminContractId = service.AdminContractId,
                        BaseServiceId = service.BaseServiceId,
                        ProviderContractId = service.ProviderContractId,
                        ProviderId = UserID
                    };

                    _context.Services.Add(newService); // Add the new service to the DbContext
                    await _context.SaveChangesAsync(); // Save the changes to the database

                    await FileUpload(Images, newService.ServiceId);
                    await ServicePrice(service.ServicePrice, newService.ServiceId);

                    return RedirectToAction(nameof(Index)); // Redirect to the list of services
                }
                catch (Exception ex)
                {
                    HandleError(ex.Message, "Services", nameof(Index));
                }
            }

            await AddSelectLists();
            return View(service);
        }

        // GET: Services/Edit/5
        [Authorize("Provider")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                HandleError("ID is Required !!!", "Services", nameof(Index));

            var service = await _context.Services
                .Where(s => s.ServiceId == id)
                .Include(s => s.Category)
                .FirstOrDefaultAsync();

            if (service == null)
                HandleError("The Service Does Not Exist !!!", "Services", nameof(Index));

            var serviceModel = new ServiceModel()
            {
                ServiceId = service.ServiceId,
                Name = service.Name,
                Details = service.Details,
                Location = service.Location,
                StartTime = service.StartTime.Hours,
                EndTime = service.EndTime.Minutes > 0 ? 24 : service.EndTime.Hours,
                Quantity = service.Quantity,
                InitialPaymentPercentage = service.InitialPaymentPercentage,
                CategoryId = service.CategoryId,
                AdminContractId = service.AdminContractId,
                BaseServiceId = service.BaseServiceId,
                ProviderContractId = service.ProviderContractId,
            };
            var price = await _context.ServicePrices
                .Where(s => s.ServiceId == id && s.PriceDate.Date == DateTime.Now.Date)
                .Select(s => s.Price)
                .FirstOrDefaultAsync();

            serviceModel.ServicePrice = ( price > 0) ? price : 1;


            await AddSelectLists(serviceModel);
            return View(serviceModel);
        }

        [HttpPost]
        [Authorize("Provider")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ServiceModel serviceModel, IFormFileCollection Images)
        {
            try
            {
                var service = await _context.Services
                    .FirstOrDefaultAsync(s => s.ServiceId == serviceModel.ServiceId);

                if (service == null)
                    HandleError("The Service Does Not Exist !!!", "Services", nameof(Index));
                
                if (ModelState.IsValid)
                {
                    service.Name = serviceModel.Name;
                    service.Details = serviceModel.Details;
                    service.Location = serviceModel.Location;
                    service.StartTime = new TimeSpan(serviceModel.StartTime, 0, 0);
                    if (serviceModel.EndTime == 24)
                        service.EndTime = new TimeSpan(serviceModel.EndTime - 1, 59, 59);
                    else
                        service.EndTime = new TimeSpan(serviceModel.EndTime, 0, 0);
                    service.Quantity = serviceModel.Quantity ?? 0;
                    service.InitialPaymentPercentage = serviceModel.InitialPaymentPercentage;
                    service.CategoryId = serviceModel.CategoryId;
                    service.AdminContractId = serviceModel.AdminContractId;
                    service.BaseServiceId = serviceModel.BaseServiceId;
                    service.ProviderContractId = serviceModel.ProviderContractId;

                    if (service.EndTime == TimeSpan.Zero || service.EndTime <= service.StartTime)
                    {
                        await AddSelectLists();
                        return View(serviceModel);
                    }

                    _context.Update(service);
                    await _context.SaveChangesAsync();

                    await FileUpload(Images, service.ServiceId);

                    return RedirectToAction(nameof(Index));
                }

                await AddSelectLists(serviceModel);
                return View(serviceModel);
            }
            catch (Exception e)
            {
                HandleError(e.Message, "Services", nameof(Index));
            }

            await AddSelectLists(serviceModel);
            return View(serviceModel);
        }

        // GET: Services/Delete/5
        [Authorize("Provider")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                HandleError("ID is Required !!!", "Services", nameof(Index));

            var service = await _context.Services.FindAsync(id);
            if (service == null)
                HandleError("The Service Dose Not Exists !!!", "Services", nameof(Index));

            return View(service);
        }

        // POST: Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize("Provider")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
                HandleError("The Service Dose Not Exists !!!", "Services", nameof(Delete));


            var hasBookings = _context.BookingServices
                .Include(bs => bs.Booking)
                .Where(b => b.Booking.EventDate > DateTime.Now)
                .Any(b => b.ServiceId == id);
            if (hasBookings)
            {
                ViewData["ServiceBooking"] = _context.BookingServices.FirstOrDefault(b => b.ServiceId == id);
                return View(service);
            }

            try
            {
                service.IsBlocked = !service.IsBlocked;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                HandleError(ex.Message, "Services", "Index");
            }
            return RedirectToAction(nameof(Index));
        }

        //GET: Services/Images/5
        [Authorize("Provider")]
        public async Task<IActionResult> GetImages(int id)
        {
            if (!ServiceExists(id))
                HandleError("The Service Dose Not Exists !!!", "Services", "Index");

            var serviceImages = _context.ServiceImages.Where(s => s.ServiceId == id).Include(s => s.Service);
            var service = await _context.Services.FindAsync(id);
            ViewData["servicename"] = service.Name;
            ViewData["serviceid"] = service.ServiceId;

            return View(await serviceImages.ToListAsync());
        }

        // GET: AddImage
        public async Task<IActionResult> AddImage(int id)
        {
            if (!ServiceExists(id))
                HandleError("The Service Dose Not Exists !!!", "Services", nameof(GetImages));

            ViewData["ServiceId"] = id;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddImage(int id, IFormFileCollection Images)
        {
            if (!ServiceExists(id))
                HandleError("Service Not Found", "Services", "GetImages");

            await FileUpload(Images, id);
            return RedirectToAction(nameof(GetImages), new { id });
        }

        public async Task<IActionResult> DeleteImage(int id, [FromQuery] string url)
        {
            if (!ServiceExists(id))
                HandleError("Id Is Requirer !!!", "Services", nameof(GetImages));

            var serviceImage = await _context.ServiceImages
                .FirstOrDefaultAsync(si => si.ServiceId == id && si.URL == url);

            if (serviceImage == null)
                HandleError("The Service Dose Not Exists !!!", "Services", nameof(GetImages));


            _context.ServiceImages.Remove(serviceImage);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(GetImages), new { id });
        }

        // GET: Prices/5
        [Authorize("Provider")]
        public async Task<IActionResult> Prices(int id)
        {
            if (!ServiceExists(id))
            {
                return HandleError("Services Does Not Exists", "Services", "Index");
            }

            var servicePrices = _context.ServicePrices.Where(s => s.ServiceId == id).Include(s => s.Service);
            var service = await _context.Services.FindAsync(id);
            ViewData["ServiceName"] = service?.Name ?? "";
            ViewData["ServiceId"] = service?.ServiceId;

            return View(await servicePrices.ToListAsync());
        }

        // GET: AddPrice
        public IActionResult AddPrice(int id)
        {
            if (!ServiceExists(id))
                return HandleError("Services Does Not Exists", "Services", "Index");

            ViewData["ServiceId"] = id;
            return View();
        }

        // POST: AddPrice
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPrice(int id, [Bind("ServiceId,PriceDate,StartDate,EndDate,Price")] ServicePriceModel servicePriceModel)
        {
            if (!ServiceExists(id))
            {
                return HandleError("Services Does Not Exists", "Services", nameof(Prices));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    if (servicePriceModel.EndDate.Date != DateTime.Now.Date) // Compare only date part
                    {
                        DateTime currentDate = servicePriceModel.StartDate;
                        while (currentDate <= servicePriceModel.EndDate)
                        {
                            var servicePrice = new ServicePrice
                            {
                                ServiceId = servicePriceModel.ServiceId,
                                Price = servicePriceModel.Price,
                                PriceDate = currentDate
                            };
                            _context.ServicePrices.Add(servicePrice);
                            currentDate = currentDate.AddDays(1); // Increment the date by 1 day
                        }
                    }
                    else
                    {
                        var servicePrice = new ServicePrice
                        {
                            ServiceId = servicePriceModel.ServiceId,
                            Price = servicePriceModel.Price,
                            PriceDate = servicePriceModel.PriceDate
                        };
                        _context.ServicePrices.Add(servicePrice);
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Prices), new { id = id });
                }

            }
            catch (Exception ex)
            {
                HandleError(ex.Message, "Services", nameof(Prices));
            }
            return View(servicePriceModel);
        }
        public async Task<IActionResult> EditPrice(int id, [FromQuery] DateTime date)
        {
            if (!ServiceExists(id))
            {
                return HandleError("The Service Does Not Exist!", "Services", nameof(Prices));
            }

            var servicePrice = await _context.ServicePrices
                .Where(sp => sp.ServiceId == id && sp.PriceDate == date)
                .FirstOrDefaultAsync();

            if (servicePrice == null)
            {
                return HandleError("The Service Price does not exist for the given date!", "Services", nameof(Prices));
            }

            ViewData["ServiceId"] = id;
            return View(servicePrice);
        }

        [HttpPost]
        public async Task<IActionResult> EditPrice(int id, [Bind("ServiceId,PriceDate,Price")] ServicePrice servicePrice)
        {
            if (!ServiceExists(id))
            {
                HandleError("The Service does not exist!", "Services", nameof(Prices));
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the existing service price from the database
                    var existingServicePrice = await _context.ServicePrices
                        .Where(sp => sp.ServiceId == id && sp.PriceDate == servicePrice.PriceDate)
                        .FirstOrDefaultAsync();

                    if (existingServicePrice == null)
                    {
                        HandleError("The Service Price does not exist for the given date!", "Services", nameof(Prices));
                        return NotFound();
                    }

                    // Update the properties of the existing service price
                    existingServicePrice.Price = servicePrice.Price;

                    // Mark the entity as modified
                    _context.ServicePrices.Update(existingServicePrice);
                    await _context.SaveChangesAsync();

                    // Redirect to a relevant page (for example, the details page of the service)
                    return RedirectToAction(nameof(Prices), new { id = id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Handle concurrency issues
                    HandleError("The service price could not be updated due to concurrency issues.", "Services", nameof(Prices));
                    return StatusCode(500);
                }
                catch (Exception ex)
                {
                    // Handle other possible exceptions
                    HandleError($"An error occurred while updating the service price: {ex.Message}", "Services", nameof(Prices));
                    return StatusCode(500);
                }
            }

            // If we got this far, something failed, redisplay the form
            return View(servicePrice);
        }

        [HttpPost]
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
                    return Json(new { success = true, message = "Request to make service online is submitted." });
                }
            }
            return Json(new { success = false, message = "Service not found." });
        }


        // Helper Method: Check if a service exists
        private bool ServiceExists(int id)
        {
            return _context.Services.Any(e => e.ServiceId == id);
        }

        // Helper Method: Populate SelectLists
        private async Task AddSelectLists(ServiceModel? service = null)
        {
            // Generate hours dictionary using a single line LINQ statement
            var StartTimehours = Enumerable.Range(0, 24)
                .ToDictionary(i => i.ToString("D2") + " :00", i => i);
            var EndTimehours = Enumerable.Range(0, 25)
                .ToDictionary(i => i.ToString("D2") + " :00", i => i);

            UserID = await GetCurrentUserID();
            var response = await _client.GetAsync(SaudiArabiaRegionsCitiesAndDistricts);
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var regions = JsonConvert.DeserializeObject<List<Region>>(jsonData);
                ViewData["Location"] = new SelectList(regions, "name_en", "name_en");
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", service?.CategoryId);
            var baseServicesQuery = _context.Services.Where(s => s.ProviderId == UserID);
            if (service != null)
            {
                baseServicesQuery = baseServicesQuery.Where(s => s.ServiceId != service.ServiceId);
            }
            ViewData["BaseServiceId"] = new SelectList(baseServicesQuery, "ServiceId", "Name", service?.BaseServiceId);
            ViewData["AdminContractId"] = new SelectList(_context.AdminContracts.Where(ac => ac.IsBlocked == false), "ContractId", "ContractName", service?.AdminContractId);
            ViewData["ProviderContractId"] = new SelectList(_context.ProviderContracts.Where(p => p.ProviderId == UserID && p.IsBlocked == false), "ContractId", "ContractName", service?.ProviderContractId);
            ViewData["StartTime"] = new SelectList(StartTimehours, "Value", "Key");
            ViewData["EndTime"] = new SelectList(EndTimehours, "Value", "Key");
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
                    if (image.Length > 0 && AllowedImageTypes.Contains(image.ContentType))
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
                    else
                    {
                        // Handle invalid files (optional: throw an exception, log, or return a message)
                        throw new InvalidDataException("Only image files are allowed.");
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
                    PriceDate = DateTime.Now.Date,
                    Price = price
                };
                _context.ServicePrices.Add(servicePrice);
                await _context.SaveChangesAsync();
            }
        }
        
        private static readonly List<string> AllowedImageTypes = new List<string>
        {
            "image/jpeg",
            "image/png",
            "image/gif",
            "image/bmp",
            "image/webp"
        };
    }
}
