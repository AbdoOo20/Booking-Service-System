using BookingServices.Data;
using BookingServices.Data.Migrations;
using BookingServices.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using System.Buffers;

namespace BookingServices.Controllers
{
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
                };
                customers.Add(customerData);
            }
            return View(customers);
        }

        public ActionResult Search(string searchType, string searchValue)
        {
            List<CustomerData> customers = new List<CustomerData>();
            List<Customer> AllCustomers;

            if (!string.IsNullOrEmpty(searchValue))
            {
                switch (searchType)
                {
                    case "Name":
                        AllCustomers = context.Customers
                            .Where(c => c.Name.ToLower().Contains(searchValue.ToLower()))
                            .Include(p => p.IdentityUser)
                            .ToList();
                        break;
                    case "SSN":
                        AllCustomers = context.Customers
                            .Where(c => c.SSN.Contains(searchValue))
                            .Include(p => p.IdentityUser)
                            .ToList();
                        break;
                    case "Phone":
                        AllCustomers = context.Customers
                            .Include(p => p.IdentityUser)
                            .Where(c => c.AlternativePhone.Contains(searchValue) || c.IdentityUser.PhoneNumber.Contains(searchValue))
                            .ToList();
                        break;
                    default:
                        AllCustomers = new List<Customer>();
                        break;
                }

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
                    };
                    customers.Add(customerData);
                }
            }
            else 
            {
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
                    };
                    customers.Add(customerData);
                }
            }
            return PartialView("CustomerList", customers);
        }
    }
}
