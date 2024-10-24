﻿using BookingServices.Data;
using CusromerProject.DTO.Customer;
using CustomerProject.DTO.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using PayPal.Api;
using SendGrid.Helpers.Mail;

namespace CusromerProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        UserManager<IdentityUser> userManager;
        public CustomerController(ApplicationDbContext _context ,UserManager<IdentityUser> _userManager)
        {
            context = _context;
            userManager = _userManager;
        }

        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<CustomerCrudDTO> GetByID(string id) 
        {
            if (id == null) return NotFound();
            var customer = context.Customers.Include(c=>c.IdentityUser).FirstOrDefault(c=>c.CustomerId == id);
            if(customer == null) return NotFound();
            CustomerCrudDTO customerData = new CustomerCrudDTO();
            customerData.CustomerId = customer.CustomerId;
            customerData.Name = customer.Name;
            customerData.Email = customer.IdentityUser.Email;
            customerData.SSN = customer.SSN;
            customerData.Phone = customer.IdentityUser.PhoneNumber;
            customerData.AlternativePhone = customer.AlternativePhone;
            customerData.City = customer.City;
            customerData.BankAccount = customer.BankAccount;
            return customerData;
        }
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(string id, CustomerCrudDTO customerData)
        {
            if (id == null) return NotFound();
            var user = await userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var customer = context.Customers.FirstOrDefault(c => c.CustomerId == id);
            if (customer == null) return NotFound();
            if (ModelState.IsValid)
            {
                /* Update User */
                user.PhoneNumber = customerData.Phone;
                user.Email = customerData.Email;
                user.NormalizedEmail = customerData.Email.ToUpper();
                /* Update Customer */
                customer.Name = customerData.Name;
                customer.AlternativePhone = customerData.AlternativePhone;
                customer.City = customerData.City;
                customer.SSN = customerData.SSN;
                customer.BankAccount = customerData.BankAccount;
                context.SaveChanges();
                return Ok(new {Message = "Updated Successfully"});
            }
            else return BadRequest(ModelState);
        }

        [HttpPut("block/{id}")]
        [Authorize]
        public IActionResult Block(string id)
        {
            if (id == null) return NotFound();
            var customer = context.Customers.FirstOrDefault(c => c.CustomerId == id);
            if (customer == null) return NotFound();
            //int haveBooking = context.Bookings.Where(b => (b.CustomerId == id) && (b.Status == "Pending")).Count();
            if(true) //haveBooking == 0
            {
                customer.IsBlocked = true;
                context.SaveChanges();
                return Ok(new { message = "Blocked Successfully" });
            }
            else 
                return BadRequest("You have book now and can not deactivate account");
        }

        [HttpPut("changePassword{id}")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(string id , ChangeCustomerPasswordDTO chPassword) 
        {
            if (id == null) return NotFound();
            var user = await userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            if (ModelState.IsValid)
            {
                var result = await userManager.ChangePasswordAsync(user, chPassword.CurrentPassword, chPassword.NewPassword);
                if (result.Succeeded) return Ok();
                else return BadRequest();
            } 
            else return BadRequest();
            
        }

        [HttpPut("SetBanckAccount/{id}")]
        [Authorize]
        public async Task<IActionResult> SetBanckAccount(string id, bankAccountDTO bankAcount)
        {
            var customer = await context.Customers.FindAsync(id);
            if (customer == null) return NotFound(new {Message = "Customer Not Found"});
            if (ModelState.IsValid)
            {
                customer.BankAccount = bankAcount.bankAccount;
                try
                {
                    context.SaveChanges();
                }
                catch (Exception)
                {
                    return BadRequest(new { Message = "Unexpected Error" });
                }
            }
            else return BadRequest(new { Message = ModelState });
            return Ok(new { Message = "Bank Account Set Successfully" });
        }

        [HttpGet("GetBanckAccount/{id}")]
        [Authorize]
        public async Task<IActionResult> GetBanckAccount(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound(new { Message = "Insert the correct data" });
            }

            var bankAccount = await context.Customers
                            .Where(c => c.CustomerId == id)
                            .Select(c => c.BankAccount)
                            .FirstOrDefaultAsync();

            if (bankAccount == null)
            {
                return NotFound(new { Message = "bankAccount Not Found" });
            }
            
            return Ok(new { bankAccount = bankAccount });
        }
    }
}
