using BookingServices.Data;
using BookingServices.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;

namespace BookingServices.Controllers
{
    public class _BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public _BookingController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var user = await _userManager.GetUserAsync(User);
            string userIdFromManager = user?.Id ?? "";
            var bookings = _context.Bookings.Include(c => c.Customer);

            if (startDate.HasValue && endDate.HasValue)
            {
                bookings = bookings.Where(b => b.EventDate >= startDate.Value && b.EventDate <= endDate.Value).Include(C => C.Customer);
            }



            var totalConfirmed = bookings
                .Where(b => b.Status == "Confirmed" || b.Status == "paid")
                .Sum(b => b.Price);


            var totalCanceled = bookings
                .Where(b => b.Status == "Cancelled" || b.Status == "pus")
                .Sum(b => b.Price);
            var totalPending = bookings
               .Where(b => b.Status == "Pending" || b.Status == "pus")
               .Sum(b => b.Price);

            var model = new bookingViewModel
            {
                Bookings = bookings.ToList(),
                TotalIncome = totalConfirmed,
                TotalCanceled = totalCanceled,
                TotalPending = totalPending
            };

            return View(model);
        }
        //this is Booking controller 

    }
}
