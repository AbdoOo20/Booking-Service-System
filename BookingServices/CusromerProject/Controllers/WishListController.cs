using BookingServices.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CusromerProject.DTO.Categories;
using CustomerProject.DTO.WishList;
using BookingServices.ViewModel;
using CusromerProject.DTO.Services;
using Microsoft.AspNetCore.Authorization;

namespace BookingServices.Controllers
{
    [Route("api/wishlist")]
    [ApiController]
    public class WishListController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WishListController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all Categories with Services in WishList by CustomerId
        [HttpGet("{customerId}")]
        //[Authorize]
        public IActionResult GetAllServicesInWishList(string customerId)
        {
            // جلب الخدمات فقط إذا كان لها سعر لليوم الحالي
            var servicesInWishList = _context.WishList
                .Where(w => w.CustomerId == customerId)
                .Include(w => w.Service)
                .ThenInclude(s => s.ServicePrices)
                .Select(w => new ServiceForCategory
                {
                    Id = w.Service.ServiceId,
                    Name = w.Service.Name,
                    Location = w.Service.Location,
                    Price = w.Service.ServicePrices
                        .Where(p => p.PriceDate.Date == DateTime.Now.Date)
                        .Select(p => p.Price)
                        .FirstOrDefault(),
                    Image = w.Service.ServiceImages.FirstOrDefault().URL
                })
                .Where(s => s.Price > 0) // عرض الخدمات التي تحتوي على سعر فقط
                .ToList();

            return Ok(servicesInWishList);
        }



        // 2. Add Service to WishList (POST)
        [HttpPost]
        //[Authorize]
        public IActionResult AddServiceToWishList([FromBody] WishListDTO wishList)
        {
            if (wishList == null || string.IsNullOrWhiteSpace(wishList.CustomerId) || wishList.ServiceId <= 0)
            {
                return BadRequest("Invalid data provided.");
            }

            // تحقق مما إذا كانت الخدمة موجودة قبل الإضافة (اختياري)
            var existingService = _context.Services.Find(wishList.ServiceId);
            if (existingService == null)
            {
                return NotFound("Service not found.");
            }

            WishList wish = new WishList() { ServiceId=existingService.ServiceId , CustomerId = wishList.CustomerId};

            _context.WishList.Add(wish);
            _context.SaveChanges();
            return Ok("Service added to wishlist successfully.");
        }

        // 3. Delete Service from WishList (DELETE)
        [HttpDelete("{customerId}/{serviceId}")]
        //[Authorize]
        public IActionResult DeleteServiceFromWishList(string customerId, int serviceId)
        {
            var wishListItem = _context.WishList
                                       .FirstOrDefault(w => w.CustomerId == customerId && w.ServiceId == serviceId);
            if (wishListItem == null)
            {
                return NotFound("Service  not found in customer's wishlist.");
            }

            _context.WishList.Remove(wishListItem);
            _context.SaveChanges();
            return Ok("Service removed from wishlist.");
        }
    }
}
