using BookingServices.Data;
using CusromerProject.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CusromerProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerRegisterController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;
        public CustomerRegisterController(ApplicationDbContext _context , UserManager<IdentityUser> _userManager)
        {
            context = _context;
            userManager = _userManager;
        }
        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerDataDTO customerData)
        {
            if (customerData == null ||
                string.IsNullOrEmpty(customerData.Username) ||
                string.IsNullOrEmpty(customerData.Password) ||
                string.IsNullOrEmpty(customerData.Name) ||
                string.IsNullOrEmpty(customerData.AlternativePhone) ||
                string.IsNullOrEmpty(customerData.SSN) ||
                string.IsNullOrEmpty(customerData.City))
            {
                return BadRequest();
            }
            var user = new IdentityUser
            {
                UserName = customerData.Username,
                Email = customerData.Email , 
                PhoneNumber = customerData.Phone
            };

            var result = await userManager.CreateAsync(user, customerData.Password);
            if (!result.Succeeded)
            {
                return BadRequest();
            }

            var customer = new Customer
            {
                CustomerId = user.Id,
                Name = customerData.Name,
                AlternativePhone = customerData.AlternativePhone,
                SSN = customerData.SSN,
                City = customerData.City,
                IsOnlineOrOfflineUser = true , 
                IsBlocked = false
            };

            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
