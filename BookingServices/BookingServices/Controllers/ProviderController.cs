using BookingServices.Data;
using BookingServices.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
//using System.Drawing;
using BookingServices.Models;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Specialized;
using NuGet.Protocol;
using System.Reflection;
using Microsoft.IdentityModel.Tokens;

namespace BookingServices.Controllers
{
    [Authorize(Roles = "Provider")]
    public class ProviderController : Controller
    {
        private ApplicationDbContext _context;
        private UserManager<IdentityUser> _userManager;
        private string SaudiArabiaRegionsCitiesAndDistricts;
        private readonly HttpClient _client;
        ErrorViewModel errorViewModel = new ErrorViewModel { Message = "", Controller = "", Action = "" };
        public ProviderController([FromServices] ApplicationDbContext context, HttpClient client, UserManager<IdentityUser> userManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager;
            SaudiArabiaRegionsCitiesAndDistricts = "https://raw.githubusercontent.com/homaily/Saudi-Arabia-Regions-Cities-and-Districts/refs/heads/master/json/regions_lite.json";
            _client = client;
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            // Fetch Regions data from external API for locations
            var response = await _client.GetAsync(SaudiArabiaRegionsCitiesAndDistricts);
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var regions = JsonConvert.DeserializeObject<List<Region>>(jsonData);
                //ViewBag.Locations = regions.Select(r => r.name_en.Trim()).Distinct().ToList(); // Ensure no duplicates
                TempData["Locations"] = regions.Select(r => r.name_en.Trim()).Distinct().ToList(); // Ensure no duplicates
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(OfflineCustomer model)
        {
            if (ModelState.IsValid)
            {
                var findcustomer = await _context.Customers.FirstOrDefaultAsync(x => x.SSN == model.SSN);
                if (findcustomer != null)
                {
                    TempData["Message"] = "Customer with this SSN already exists!";
                    TempData["MessageType"] = "error";
                    return RedirectToAction("Register");
                }

                var findcustomeralternatphone = await _context.Customers.FirstOrDefaultAsync(x => x.AlternativePhone == model.AlternativePhone);
                if (findcustomeralternatphone != null)
                {
                    TempData["Message"] = "Customer with this Alternative Phone already exists!";
                    TempData["MessageType"] = "error";
                    return RedirectToAction("Register");
                }
                var findcustomerphone = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == model.Phone);
                if (findcustomerphone != null)
                {
                    TempData["Message"] = "Customer with this Phone already exists!";
                    TempData["MessageType"] = "error";
                    return RedirectToAction("Register");
                }


                string tt = DateTime.Now.ToString();
                string _TempUserNameAndPassword = "";
                foreach (var x in tt) if (char.IsLetterOrDigit(x)) _TempUserNameAndPassword += x;

                var user = new IdentityUser
                {
                    UserName = _TempUserNameAndPassword,
                    PhoneNumber = model.Phone,
                    EmailConfirmed = false,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0
                };

                var result = await _userManager.CreateAsync(user, "EW358@gmail.com");
                if (result.Succeeded)
                {
                    _context.Customers.Add(
                        new Customer()
                        {
                            CustomerId = user.Id,
                            Name = model.Name,
                            AlternativePhone = model.AlternativePhone,
                            SSN = model.SSN,
                            City = model.City
                        });
                    await _context.SaveChangesAsync();

                    // when is the registeration successfull go to the show all services to the providor
                    TempData["Message"] = "Registration successful!";
                    TempData["MessageType"] = "success";
                    return RedirectToAction("Register");
                }

                TempData["Message"] = "Registration failed. Please try again.";
                TempData["MessageType"] = "error";
                foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
                return Json(ModelState);
            }
            return View(model);
        }


        // Provider/Booking?id=1
        private static int SharedserviceId;
        [HttpGet]
        public async Task<IActionResult> Booking(int id)
        {
            try
            {
                var today = DateTime.Now.Date;
                SharedserviceId = id;
                ViewBag.AvailableQuantity = GetQuantity(id, today);

                var Name = await _context.Services
                    .Where(s => s.ServiceId == id)
                    .Select(s => s.Name).FirstOrDefaultAsync();
                ViewBag.ServiceName = Name?.ToString();

                ViewBag.paymentMethod = await _context.PaymentIncomes.ToListAsync();

                ViewBag.priceOfCurrentDay = await _context.ServicePrices
                    .Where(s => s.ServiceId == id && s.PriceDate == DateTime.Now.Date)
                    .Select(s => s.Price).FirstOrDefaultAsync();

                ViewBag.ServiceId = id;
                var service = await _context.Services.FindAsync(id);
                if (service == null)
                {
                    TempData["Message"] = "Service not found.";
                    TempData["MessageType"] = "error";
                    return RedirectToAction("Index");
                }
                
                TempData["serviceQuentity"] = service.Quantity;
                return View();
            }
            catch (Exception e)
            {
                errorViewModel = new ErrorViewModel { Message = e.Message/*"An unexpected error occurred. Please try again later."*/, Controller = " Admin", Action = "Index" };
                return View("Error", errorViewModel);
            }

        }

        private string sharedSSN = "";
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Booking(Booking model)
        {
            try
            {
                model.Status = "paid";
                model.Type = "Service";
                if (ModelState.IsValid)
                {

                    var isOverlapping = _context.Bookings.Where(b => b.EventDate == model.EventDate && b.Status != "canceled")  // Only check for the same day and exclude canceled bookings
                                        .Any(b => (model.StartTime < b.EndTime && model.EndTime > b.StartTime));  // Check if times overlap
                    if (isOverlapping)
                    {
                        TempData["Message"] = "The selected time slot is already booked by another user.";
                        TempData["MessageType"] = "error";
                        return View(model);
                    }

                    var customer = await _context.Customers.FirstOrDefaultAsync(x => x.SSN == model.CustomerId);
                    if (customer == null)
                    {
                        TempData["Message"] = "Customer not found.";
                        TempData["MessageType"] = "error";
                        return RedirectToAction("Booking", new { serviceId = SharedserviceId });
                    }

                    var bookingEntity = new Booking
                    {
                        EventDate = model.EventDate,
                        StartTime = model.StartTime,
                        EndTime = model.EndTime,
                        Status = model.Status, // allow null -- not in default pendding
                        Quantity = model.Quantity,
                        Price = model.Price, // just for view -- not in from the service price
                        CashOrCashByHandOrInstallment = model.CashOrCashByHandOrInstallment, // or cash by hand
                        BookDate = DateTime.Now, // -- not in default date.now
                        Type = model.Type, // allow null -- service
                        CustomerId = _context.Customers.FirstOrDefault(x => x.SSN == model.CustomerId).CustomerId, // allow null
                        PaymentIncomeId = model.PaymentIncomeId,
                    };

                    _context.Bookings.Add(bookingEntity);
                    _context.SaveChanges();

                    _context.BookingServices.Add(new BookingService() { BookingId = bookingEntity.BookingId, ServiceId = SharedserviceId });

                    _context.SaveChanges();

                    // from this point i will go to pay
                    //return Content("All is will bro, Booking");
                    TempData["Message"] = "Booking successful!";
                    TempData["MessageType"] = "success";
                    return RedirectToAction("Booking", new { serviceId = SharedserviceId });
                }
                TempData["Message"] = "Booking failed. Please try again.";
                TempData["MessageType"] = "error";
                return View(model);
            }
            catch (Exception e)
            {
                errorViewModel = new ErrorViewModel { Message = "An unexpected error occurred. Please try again later.", Controller = " Admin", Action = "Index" };
                return View("Error", errorViewModel);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetCustomerBySSN(string ssn)
        {
            // dont forget this is an offline customer
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.SSN == ssn && c.IsOnlineOrOfflineUser == false);
            if (customer != null)
            {
                return Json(new { success = true, customerName = customer.Name });
            }

            return Json(new { success = false });
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableTimes(int serviceId, DateTime eventDate)
        {
            var availableTimes = await GetTheAvailableTime(serviceId, eventDate);
            if (availableTimes.IsNullOrEmpty())
            {
                TempData["Message"] = "Booking Completed in this Day";
                TempData["MessageType"] = "success";
            }
            return Json(availableTimes);
        }

        [HttpGet]
        public async Task<IActionResult> GetEndTimes(int serviceId, DateTime eventDate, string startTime)
        {
            var availableTimes = await GetTheAvailableTime(serviceId, eventDate);
            var availableEndTime = new List<string>() { startTime.Substring(0, 2) };
            for (int i = 0; i < availableTimes.Count; i++)
            {
                if (int.Parse(availableTimes[i]) - int.Parse(availableEndTime.Last()) == 1)
                {
                    availableEndTime.Add(availableTimes[i]);
                }
            }
            var lasthour = int.Parse(availableEndTime.Last()) + 1;
            availableEndTime.Add(lasthour.ToString());
            availableEndTime.RemoveAt(0);
            return Json(availableEndTime);
        }


        private async Task<List<string>> GetTheAvailableTime(int serviceId, DateTime eventDate)
        {
            var StartEndTime = await _context.Services.Where(x => x.ServiceId == serviceId).Select(x => new { id = x.ServiceId, from = x.StartTime, to = x.EndTime }).ToListAsync();
            var From = StartEndTime[0].from.Hours;
            if (eventDate.Date == DateTime.Now.Date)
            {
                From = DateTime.Now.Hour;
            }
            var allTimes = new List<string>();
            for (int i = From; i < StartEndTime[0].to.Hours; i++)
                allTimes.Add(i.ToString());

            var result = await (from b in _context.Bookings
                          join bs in _context.BookingServices on b.BookingId equals bs.BookingId
                          where bs.ServiceId == serviceId && b.EventDate == eventDate && b.Status != "canceled"
                                select new { b.StartTime, b.EndTime }).ToListAsync();

            var alltimebooked = result.Select(r => (r.StartTime, r.EndTime)).ToList();
            foreach (var book in alltimebooked)
                for (int i = book.StartTime.Hour; i <= book.EndTime.Hour; i++)
                    allTimes.Remove(i.ToString());
            
            return allTimes;
        }




        private int GetAllquantity(int serviceId, DateTime eventDate)
        {
            var result = (from b in _context.Bookings
                          join bs in _context.BookingServices on b.BookingId equals bs.BookingId
                          where bs.ServiceId == serviceId && b.EventDate == eventDate
                          select b.Quantity).ToList();

            int bookingQuantity = 0;
            foreach (var book in result) bookingQuantity += book;
            return bookingQuantity;
        }

        private int GetQuantity(int serviceId, DateTime eventDate)
        {
            var quantityAvailable = _context.Services
                .Where(s => s.ServiceId == serviceId)
                .Select(s => s.Quantity).FirstOrDefault() - GetAllquantity(serviceId, eventDate);

            //var price = _context.ServicePrices
            //    .Where(x => x.ServiceId == serviceId && x.PriceDate.Date == eventDate.Date)
            //    .Select(x => x.Price).FirstOrDefault();

            return quantityAvailable;//new Tuple<int, decimal>(quantityAvailable, price);
        }

        private int GetQuantityint(int serviceId, DateTime eventDate)
        {
            return GetQuantity(serviceId, eventDate);
        }

        [HttpGet]
        public IActionResult GetQuantityout(int serviceId, DateTime eventDate)
        {
            var result = GetQuantity(serviceId, eventDate);
            return Json(new
            {
                success = true,
                quantity = result
            });
        }
    }
}


