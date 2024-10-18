using BookingServices.Data;
using BookingServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;

namespace BookingServices.Controllers
{
    [Authorize(Roles = "Provider")]
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
            List<BookingViewModel> books = new List<BookingViewModel>();
            var bookings = (from b in _context.Bookings
                            from bs in _context.BookingServices
                            from s in _context.Services
                            from c in _context.Customers
                            from m in _context.PaymentIncomes
                            where b.BookingId == bs.BookingId
                            && bs.ServiceId == s.ServiceId
                            && s.ProviderId == userIdFromManager && b.CustomerId == c.CustomerId && b.PaymentIncomeId == m.PaymentIncomeId
                            select new 
                            {
                                BookingId = b.BookingId,
                                EventDate = b.EventDate,
                                Quantity = b.Quantity,
                                Price = b.Price,
                                BookDate = b.BookDate,
                                Status = b.Status,
                                Type = b.Type,
                                PaymentIncome = b.PaymentIncomeId != null ? m.Name : b.CashOrCashByHandOrInstallment,
                                CustomerName = c.Name,
                                ServiceName = s.Name,
                            });
            foreach (var item in bookings)
            {
                BookingViewModel newBook = new BookingViewModel
                {
                    BookingId = item.BookingId,
                    EventDate = item.EventDate, 
                    Quantity = item.Quantity,
                    Price = item.Price,
                    BookDate = item.BookDate,
                    Type = item.Type,   
                    CustomerName= item.CustomerName,
                    PaymentIncome = item.PaymentIncome,
                    Status = item.Status,
                    ServiceName = item.ServiceName, 
                };
                books.Add(newBook);
            }
            return View(books);
        }
    }
}
