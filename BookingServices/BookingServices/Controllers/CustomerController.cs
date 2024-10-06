using BookingServices.Data;
using BookingServices.Data.Migrations;
using BookingServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using System.Buffers;

namespace BookingServices.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CustomerController : Controller
    {
        ApplicationDbContext context;
        ErrorViewModel errorViewModel = new ErrorViewModel { Message = "", Controller = "", Action = "" };

        public CustomerController([FromServices] ApplicationDbContext _context)
        {
            context = _context;
        }

        public ActionResult Index()
        {
            List<CustomerData> customers = new List<CustomerData>();
            List<Customer> AllCustomers = new List<Customer>();
			AllCustomers = context.Customers.Include(p => p.IdentityUser).ToList();
			foreach (var item in AllCustomers)
            {
                CustomerData customerData = new CustomerData()
                {
                    Name = item.Name,
                    City = item.City,
                    SSN = item.SSN,
                    AlternativePhone = item.AlternativePhone,
                    CustomerId = item.CustomerId,
                    IsOnlineOrOfflineUser = item.IsOnlineOrOfflineUser,
                    Phone = item.IdentityUser.PhoneNumber,
                    IsBlocked = item.IsBlocked,
                };
                customers.Add(customerData);
            }
            return View(customers);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleBlock(string id)
        {
            var customer = await context.Customers.FindAsync(id);
            if (customer == null)
            {
                return Json(new { success = false, message = "Customer Not Found" });
            }
            customer.IsBlocked = !(customer.IsBlocked ?? false);
            await context.SaveChangesAsync();
            return Json(new { success = true, isBlocked = customer.IsBlocked });
        }
    }
}
