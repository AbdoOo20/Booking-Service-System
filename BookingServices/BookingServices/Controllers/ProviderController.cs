using BookingServices.Data;
using BookingServices.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingServices.Controllers
{
    public class ProviderController : Controller
    {
        private ApplicationDbContext _context;
        private UserManager<IdentityUser> _userManager;
        public ProviderController([FromServices] ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
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

                var result = await _userManager.CreateAsync(user, _TempUserNameAndPassword + "a@");
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


        // Provider/Booking?serviceId=1
        private static int SharedserviceId;
        [HttpGet]
        public IActionResult Booking(int id)
        {
            SharedserviceId = id;
            ViewBag.AvailableQuantity = _context.Services
                .Where(s => s.ServiceId == id)
                .Select(s => s.Quantity).FirstOrDefault();

            ViewBag.ServiceName = _context.Services
                .Where(s => s.ServiceId == id)
                .Select(s => s.Name).FirstOrDefault().ToString();

            ViewBag.paymentMethod = _context.PaymentIncomes.ToList();
            
            var today = DateTime.Now.Date;
            ViewBag.priceOfCurrentDay = _context.ServicePrices
                .Where(x => x.ServiceId == id && x.PriceDate.Date == today)
                .Select(x => x.Price).FirstOrDefault();
            ViewBag.ServiceId = id;
            return View();
        }

        private string sharedSSN = "";
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Booking(Booking model)
        {
            model.Status = "pendding";
            model.Type = "Service";
            //SharedserviceId = 1;
            if (ModelState.IsValid)
            {
                var customer = _context.Customers.FirstOrDefault(x => x.SSN == model.CustomerId);
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
                    CashOrInstallment = model.CashOrInstallment, // or cash by hand
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


        [HttpGet]
        public IActionResult GetCustomerBySSN(string ssn)
        {
            // dont forget this is an offline customer
            var customer = _context.Customers.FirstOrDefault(c => c.SSN == ssn && c.IsOnlineOrOfflineUser == false);
            if (customer != null)
            {
                return Json(new { success = true, customerName = customer.Name });
            }
            return Json(new { success = false });
        }

        [HttpGet]
        public IActionResult GetAvailableTimes(int serviceId, DateTime eventDate)
        {
            return Json(GetTheAvailableTime(serviceId, eventDate));
        }

        [HttpGet]
        public IActionResult GetEndTimes(int serviceId, DateTime eventDate, string startTime)
        {
            var availableTimes = GetTheAvailableTime(serviceId, eventDate);
            var availableEndTime = new List<string>() { startTime.Substring(0, 2) };
            for (int i = 0; i < availableTimes.Count; i++)
            {
                if (int.Parse(availableTimes[i]) - int.Parse(availableEndTime.Last()) == 1)
                {
                    availableEndTime.Add(availableTimes[i]);
                }
            }
            availableEndTime.RemoveAt(0);
            return Json(availableEndTime);
        }

        private List<string> GetTheAvailableTime(int serviceId, DateTime eventDate)
        {
            var StartEndTime = _context.Services.Select(x => new { id = x.ServiceId, from = x.StartTime, to = x.EndTime }).Where(x => x.id == serviceId).ToList();
            var allTimes = new List<string>();
            for (int i = StartEndTime[0].from.Hours; i <= StartEndTime[0].to.Hours; i++)
            {
                allTimes.Add(i.ToString());
            }

            var result = (from b in _context.Bookings
                                 join bs in _context.BookingServices on b.BookingId equals bs.BookingId
                                 where bs.ServiceId == serviceId && b.EventDate == eventDate
                                 select new { b.StartTime, b.EndTime }).ToList();

            var alltimebooked = result.Select(r => (r.StartTime, r.EndTime)).ToList();
            var allTimesBookedInOneDay = new List<string>();
            foreach (var book in alltimebooked)
            {
                for (int i = book.StartTime.Hour; i <= book.EndTime.Hour; i++)
                {
                    allTimesBookedInOneDay.Add(i.ToString());
                }
            }

            return allTimes.Except(allTimesBookedInOneDay).ToList();
        }

        //private int GetAllquantity(int serviceId, DateTime eventDate)
        //{
        //    var result = (from b in _context.Bookings
        //                  join bs in _context.BookingServices on b.BookingId equals bs.BookingId
        //                  where bs.ServiceId == serviceId && b.EventDate == eventDate
        //                  select b.Quantity).ToList();

        //    int bookingQuantity = 0;
        //    foreach (var book in result) bookingQuantity += book;
        //    return bookingQuantity;
        //}
    }
}
